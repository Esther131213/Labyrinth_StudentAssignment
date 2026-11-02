using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Student.Scripts.Grid_Scripts
{
    public struct QueueItem
    {
        public Vector2Int Position;
        public float Priority;

        public QueueItem(Vector2Int position, float priority)
        {
            Position = position;
            Priority = priority;
        }
    }

    public class MinPriorityQueue
    {
        private List<QueueItem> heap = new List<QueueItem>();

        public int Count
        {
            get { return heap.Count; }
        }

        // Add an item to the queue.
        public void Enqueue(Vector2Int position, float priority)
        {
            QueueItem item = new QueueItem(position, priority);

            heap.Add(item);

            int index = heap.Count - 1;

            // Move the new item upwards until heap order is restored.
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;

                if (heap[parentIndex].Priority <= heap[index].Priority)
                    break;

                Swap(index, parentIndex);

                index = parentIndex;
            }
        }

        // Remove and return the item with the lowest priority.
        public QueueItem Dequeue()
        {
            QueueItem result = heap[0];

            int lastIndex = heap.Count - 1;

            heap[0] = heap[lastIndex];
            heap.RemoveAt(lastIndex);

            // Move the new root down until heap order is restored.
            int index = 0;

            while (true)
            {
                int leftChild = index * 2 + 1;
                int rightChild = index * 2 + 2;
                int smallest = index;

                if (leftChild < heap.Count &&
                    heap[leftChild].Priority < heap[smallest].Priority)
                {
                    smallest = leftChild;
                }

                if (rightChild < heap.Count &&
                    heap[rightChild].Priority < heap[smallest].Priority)
                {
                    smallest = rightChild;
                }

                if (smallest == index)
                    break;

                Swap(index, smallest);

                index = smallest;
            }

            return result;
        }

        private void Swap(int a, int b)
        {
            QueueItem temp = heap[a];
            heap[a] = heap[b];
            heap[b] = temp;
        }
    }
}
