using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A
{
    public int x;
    public A(int x) { this.x = x; }

    public void Act() { Debug.LogFormat("A Move\nx = {0}",x); }
}

public class B : A
{
    public int y;
    public B(int x, int y) : base(x) { this.y = y; }
    public new void Act() { Debug.LogFormat("B Move\nx = {0}\ny = {1}",x,y); }

}

public class test : MonoBehaviour
{
    public A a = new A(1);
    public B b = new B(1,2);

    public List<A> list = new List<A>();

    private void Start()
    {
        list.Add(a);
        list.Add(b);
        Debug.Log("AのAct()を実行します。");
        a.Act();
        Debug.Log("BのAct()を実行します。");
        b.Act();
    }
}