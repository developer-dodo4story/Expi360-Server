using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameRoomServer
{
    public delegate void Task();

    public class TaskExecutorScript : MonoBehaviour
    {
        private Queue<Task> TaskQueue = new Queue<Task>();
        private object queueLock = new object();

        // Update is called once per frame
        void Update()
        {
            lock (queueLock)
            {
                if (TaskQueue.Count > 0)
                    TaskQueue.Dequeue()();
            }
        }

        public void ScheduleTask(Task newTask)
        {
            lock (queueLock)
            {
                //if (TaskQueue.Count < 100) // zabezpieczenie aby zbyt nie zwiesic unity
                    TaskQueue.Enqueue(newTask);
            }
        }
    }
}
