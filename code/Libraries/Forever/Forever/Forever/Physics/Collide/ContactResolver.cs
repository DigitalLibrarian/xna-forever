using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Forever.Interface;
using Forever.Physics;


namespace Forever.Physics.Collide
{
    public class ContactResolver
    {
        public float PenetrationEpsilon = 0.000025f;
        public float VelocityEpisilon = 0.00001f;
        public int PositionIterations = 2;
        public int VelocityIterations = 1;

        [EntityInspector("Position Iterations: ")]
        public long TotalPositionIterationsExecuted { get; set; }
        [EntityInspector("Velo Iterations: ")]
        public long TotalVelocityIterationsExecuted { get; set; }
        [EntityInspector("Overall Highest Desired Delta Velocity: ")]
        public float HighestDDL { get; set; }
        [EntityInspector("Overall Highest Penetration: ")]
        public float HighestPenetration { get; set; }


        public void FullContactResolution(List<Contact> contacts, float duration)
        {

            UpdateContacts(contacts, duration); //prepare all contacts

            AdjustPositions(contacts, duration);

            AdjustVelocities(contacts, duration);
        }


        private void UpdateContacts(List<Contact> contacts, float duration)
        {
            foreach (Contact contact in contacts)
            {
               contact.ReCalc(duration);
            }

        }
   

        private void AdjustPositions(List<Contact> contacts, float duration)
        {
            int positionIterations = PositionIterations;
            int iterationsUsed = 0;
            Vector3[] linearChange = new Vector3[2];
            Vector3[] angularChange = new Vector3[2];

            while (iterationsUsed < positionIterations)
            {
                Contact primeContact = FindHighPenetration(contacts);
                if (primeContact == null)
                {
                    break;

                }

                
                primeContact.MatchAwakeState();

                primeContact.ApplyPositionChange(ref linearChange, ref angularChange, primeContact.Penetration);
                
                
                Vector3 deltaPosition;
                foreach (Contact contact in contacts)
                {
                    for (int b = 0; b < 2; b++)
                    {
                        if (contact.Bodies[b] != null)
                        {
                            for (int d = 0; d < 2; d++)
                            {
                                if (contact.Bodies[b] == primeContact.Bodies[d])
                                {
                                    
                                    deltaPosition = linearChange[d] 
                                        + TrickyMathHelper.VectorProduct(angularChange[d], contact.RelativeContactPositions[b]);
                                       
                                    contact.Penetration += TrickyMathHelper.ScalarProduct(deltaPosition, contact.Normal  ) * (b == 0 ? -1f : 1f);
                                }
                            }
                        }
                    }
                }

                iterationsUsed++;
            }
            this.TotalPositionIterationsExecuted += iterationsUsed;

        }

        private void AdjustVelocities(List<Contact> contacts, float duration)
        {
            Vector3[] velocityChange = new Vector3[2];
            Vector3[] rotationChange = new Vector3[2];
            
            int velocityIterations = VelocityIterations;
            int numIterations = 0;

            while (numIterations < velocityIterations)
            {
                Contact primeContact = FindHighDesiredDeltaVelocity(contacts);

                if (primeContact == null)
                {
                    break; ;
                }

                
                primeContact.MatchAwakeState();
                primeContact.ApplyVelocityChange(ref velocityChange, ref rotationChange);
                
                
                Vector3 deltaVel = new Vector3();
                foreach (Contact contact in contacts)
                {
                    for (int b = 0; b < 2; b++)
                    {
                        if (contact.Bodies[b] != null)
                        {
                            for (int d = 0; d < 2; d++)
                            {
                                if (contact.Bodies[b] == primeContact.Bodies[d])
                                {

                                    deltaVel = velocityChange[d]
                                        + TrickyMathHelper.VectorProduct(rotationChange[d], contact.RelativeContactPositions[b]);

                                    contact.ContactVelocity +=
                                        Vector3.Transform(deltaVel, Matrix.Transpose(contact.ContactWorld)) * (b == 0 ? -1 : 1);
                                    
                                }

                            }
                        }
                        
                    }
                 
                }

                numIterations++;
            }

            this.TotalVelocityIterationsExecuted += numIterations;
        }

        private Contact FindHighPenetration(List<Contact> contacts)
        {
            float maxPen = PenetrationEpsilon;
            Contact winner = null;
            foreach (Contact contact in contacts)
            {
                if (contact.Penetration > maxPen)
                {
                    maxPen = contact.Penetration;
                    winner = contact;
                }
            }

            if (maxPen > this.HighestPenetration)
            {
                this.HighestPenetration = maxPen;
            }

            return winner;
        }

        private Contact FindHighDesiredDeltaVelocity(List<Contact> contacts)
        {
            float maxDDV = VelocityEpisilon;
            Contact winner = null;
            foreach (Contact contact in contacts)
            {
                if (contact.DesiredDeltaVelocity > maxDDV)
                {
                    maxDDV = contact.DesiredDeltaVelocity;
                    winner = contact;
                }
            }

            if (maxDDV > this.HighestDDL)
            {
                this.HighestDDL = maxDDV;
            }


            return winner;
        }




    }
}
