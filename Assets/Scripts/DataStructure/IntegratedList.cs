using System.Collections;
using Mono.Cecil.Cil;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CmpLinkedList {
    public class IntegratedList<T> : System.Collections.IEnumerable {
        protected Node<T> head;
        public int Count { get; private set; } = 0;

        public T this[int index] {
            get {
                Node<T> current = head;
                for (int i=0; i<index; i++)
                    current = current.next;
                return current.value;
            }
            set {
                Node<T> current = head;
                for (int i=0; i<index; i++)
                    current = current.next;
                current.value = value;
            }
        }
        public void Push(T value) {
            Node<T> newNode = new Node<T>(value);

            if (head == null)
                head = newNode;
            else {
                Node<T> temp = head;
                while (temp.next != null) {
                    temp = temp.next;
                }
                temp.next = newNode;
            }
            Count++;
        }

        public void Unshift(T value) {
            Node<T> newNode = new Node<T>(value);

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
                Node<T> temp = null;
                Node<T> current = head;
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
                Node<T> current = head;
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

            Node<T> current = head;
            Node<T> temp = null;
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
            Node<T> temp = null;
            Node<T> current = head;
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
        // public T GetRandom() {
        //     Node<T> temp = head;
        //     for (int i = 0; i < Random.Range(0, count); i++) {
        //         temp = temp.next;
        //     }
        //     return temp.value;
        // }
        public T Pop() {
            if(Count <= 0)
                throw new EmptyReferenceException();
                
            Node<T> current;
            Node<T> temp;
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
            Node<T> current = head;
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
            
            Node<T> current = head;
            while(current != null) {
                if(current.value.Equals(target)) {
                    return true;
                }
                current = current.next;
            }
            return false;
        }

        public IEnumerator GetEnumerator() {
            Node<T> current = head;
            while(current != null) {
                yield return current.value;
                current = current.next;
            }
        }
    }

    public class Node<T> {
        public T value;
        public Node<T> next;
        
        public Node(T newNodeData)
        {
            this.value = newNodeData;
            this.next = null;
        }
    }
    public class EmptyReferenceException : System.Exception {}
}