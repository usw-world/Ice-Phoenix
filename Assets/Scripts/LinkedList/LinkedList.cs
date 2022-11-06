using Mono.Cecil.Cil;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CmpLinkedList
{
    public abstract class LinkedList<T>
    {
        Node<T> head;
        public int count { get; private set; } = 0;

        public T this[int index]
        {
            get
            {
                Node<T> tmpNode = head;

                for (int i = 0; i < index; i++)
                    tmpNode = tmpNode.next;

                return tmpNode.nodeData;
            }
        }

        protected void InsertTailNode(T newNodeData)
        {
            Node<T> newNode = new Node<T>(newNodeData);

            if (head == null)
                head = newNode;
            else
            {
                Node<T> tmpNode = head;

                while (tmpNode.next != null)
                {
                    tmpNode = tmpNode.next;
                }
                tmpNode.next = newNode;
            }
            count++;
        }

        protected void InsertHeadNode(T newNodeData)
        {
            Node<T> newNode = new Node<T>(newNodeData);

            if (head == null)
                head = newNode;
            else
            {
                newNode.next = head;
                head = newNode;
            }
            count++;
        }

        protected void DeleteHeadNode()
        {
            if (head.next == null)
                head = null;
            else
            {
                Node<T> tmpNode = head;
                head = tmpNode.next;
                tmpNode = null;
            }
            count--;
        }

        protected void DeleteTailNode()
        {
            while (head.next.next != null)
            {
                head = head.next;
            }
            head.next = null;

            count--;
        }

        protected T GetRandomNodeData()
        {
            Node<T> tmpNode = head;
            for (int i = 0; i < Random.Range(0, count); i++)
            {
                tmpNode = tmpNode.next;
            }

            return tmpNode.nodeData;
        }

        protected Node<T> FindNode(T data)
        {
            Node<T> tmpNode = head;
            while (tmpNode.next != null && !tmpNode.nodeData.Equals(data))
            {
                tmpNode = tmpNode.next;
            }

            return tmpNode;
        }

        protected T FindNodeData(T data)
        {
            Node<T> tmpNode = head;
            while (tmpNode.next != null && !tmpNode.nodeData.Equals(data))
            {
                tmpNode = tmpNode.next;
            }


            return tmpNode.nodeData;
        }
    }

    public class Node<T>
    {
        public T nodeData;
        public Node<T> next;

        public Node(T newNodeData)
        {
            this.nodeData = newNodeData;
            this.next = null;
        }

        public Node() { }
    }
}




