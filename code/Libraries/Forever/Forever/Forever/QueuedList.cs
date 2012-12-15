using System;
using System.Collections.Generic;
using System.Text;

namespace Forever
{
  /*
   * This class is used when you want to iterate over a list
   * at any time without having to worry about inserting during
   * the loop and blowing up your iterator.  NOTE - you can't add or remove
   * from the list unless Update() is called after.  This simply implements
   * a queued entrance and queued exit from the list.
   * */
  public class QueuedList<T> : List<T>
  {
    Queue<T> additions = new Queue<T>();
    Queue<T> removals = new Queue<T>();

    [EntityInspector("QueuedList")]
    private string StringView
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Insertions : " + additions.Count);
            sb.AppendLine("# Members : " + base.Count);
            sb.AppendLine("# Removals : " + removals.Count);
            return sb.ToString();
        }
    }

    public new void Add(T item)
    {
        this.
      additions.Enqueue(item);
    }

    public new void Remove(T item)
    {
      removals.Enqueue(item);
    }

    public void Update()
    {
      /* Empty our queues for additions
       * and removal requests.
       * */

      foreach (T item in additions)
      {
        base.Add(item);
      }
      additions.Clear();

      foreach (T item in removals)
      {
        base.Remove(item);
      }
      removals.Clear();
    }
      
  }
}
