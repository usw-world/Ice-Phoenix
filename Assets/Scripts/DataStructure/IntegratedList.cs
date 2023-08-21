using System.Collections;
using Mono.Cecil.Cil;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CmpLinkedList {
    public class IntegratedList<T> : System.Collections.IEnumerable {
        protected Node head;
        public int Count { get; private set; } = 0;
        public T this[int index] {
            get {
                Node current = head;
                for (int i=0; i<index; i++)
                    current = current.next;
                return current.value;
            }
            set {
                Node current = head;
                for (int i=0; i<index; i++)
                    current = current.next;
                current.value = value;
            }
        }
        public void Push(T value) {
            Node newNode = new Node(value);

            if (head == null)
                head = newNode;
            else {
                Node temp = head;
                while (temp.next != null) {
                    temp = temp.next;
                }
                temp.next = newNode;
            }
            Count++;
        }
        public void Unshift(T value) {
            Node newNode = new Node(value);

            if (head == null)
                head = newNode;
            else {
                newNode.next = head;
                head = newNode;
            }
            Count++;
        }
        public void Remove(int index) {
            if(index >= Count)
                return;
            
            if(index == 0) {
                head = head.next;
                Count --;
            } else {
                Node temp = null;
                Node current = head;
                for(int i=0; i<index; i++) {
                    temp = current;
                    current = current.next;
                }
                temp.next = current.next;
                Count --;
            }
        }
        public void Remove(T target) {
            if(Count <= 0)
                return;

            if(Count <= 1) {
                if(head.value.Equals(target)) {
                    Remove(0);
                }
            } else {
                int n = 0;
                Node current = head;
                while(current != null) {
                    if(current.value.Equals(target)) {
                        Remove(n);
                        return;
                    }
                    n ++;
                    current = current.next;
                }
            }
        }
        public T Shift() {
            if(Count <= 0)
                throw new EmptyReferenceException();

            T res;
            if (head.next == null) {
                res = head.value;
                head = null;
            }
            else {
                res = head.value;
                head = head.next;
            }
            Count--;
            return res;
        }
        public T Shift(int index) {
            if(Count <= 0)
                throw new EmptyReferenceException();
                
            if(index == 0) {
                return Shift();
            }

            Node current = head;
            Node temp = null;
            for(int i=0; i<index; i++) {
                temp = current;
                current = current.next;
                if(current == null)
                    throw new System.OutOfMemoryException();
            }
            temp.next = current.next;
            Count--;
            return current.value;
        }
        public bool Shift(T target, out T data) {
            // if(Count <= 0)
            //     throw new EmptyReferenceException();
            Node temp = null;
            Node current = head;
            while(current != null) {
                if(current.value.Equals(target)) {
                    data = current.value;
                    if(temp == null) head = current.next;
                    else temp.next = current.next;
                    Count--;
                    return true;
                }
                temp = current;
                current = current.next;
            }
            data = default(T);
            return false;
        }
        public T Pop() {
            if(Count <= 0)
                throw new EmptyReferenceException();
                
            Node current;
            Node temp;
            if(Count < 2) {
                temp = head;
                head = null;
                return temp.value;
            } else {
                temp = head;
                current = head.next;
                while (current.next != null) {
                    temp = current;
                    current = current.next;
                }
                temp.next = null;

                Count--;
                return current.value;
            }
        }
        public bool Find(T target, out T data) {
            Node current = head;
            while (current != null) {
                if(current.value.Equals(target)) {
                    data = current.value;
                    return true;
                }
                current = current.next;
            }
            data = default(T);
            return false;
        }
        public bool Contains(T target) {
            if(Count <= 0)
                return false;
            
            Node current = head;
            while(current != null) {
                if(current.value.Equals(target)) {
                    return true;
                }
                current = current.next;
            }
            return false;
        }
        public IEnumerator GetEnumerator() {
            Node current = head;
            while(current != null) {
                yield return current.value;
                current = current.next;
            }
        }
        public IntegratedList<T> Copy() {
            IntegratedList<T> res = new IntegratedList<T>();
            foreach(T item in this) {
                res.Push(item);
            }
            return res;
        }

        protected class Node {
            public T value;
            public Node next;
            public Node(T newNodeData) {
                this.value = newNodeData;
                this.next = null;
            }
        }
    }
    public class EmptyReferenceException : System.Exception {}
}