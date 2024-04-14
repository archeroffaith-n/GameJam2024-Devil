using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;

public class OrdersManager : MonoBehaviour
{
    public class Order 
    {
        public enum OrderType
        {
            Soul,
            Money
        }

        public enum OrderStatus
        {
            Calling,
            Answered,
            Accepted,
            Finished,
            Missed,
            Aborted
        }

        public class StatusWithTime
        {
            public OrderStatus status;
            public float time;

            public StatusWithTime(OrderStatus status, float time)
            {
                this.status = status;
                this.time = time;
            }
        }

        // Calling -> Answered -> Accepted -> Finished
        //         \           \           \      
        //          -> Missed   -> Aborted <-

        public int id;
        public int phoneLine;
        public OrderType orderType;
        public int orderValue;
        public bool isCallCorrect;
        public List<StatusWithTime> statusList;
        public float? resultQuality;
        public float? rating; // Нужно снижать и повышать рейтинг на основе каких-то событий, типа пороги всякие и т.п. По факту, можно рассчитывать его просто в конце

        public Order(int id, int phoneLine, OrderType orderType, bool isCallCorrect, int? orderValue_opt)
        {
            statusList = new List<StatusWithTime>();
            ChangeStatus(OrderStatus.Calling);
            this.id = id;
            this.phoneLine = phoneLine;
            this.orderType = orderType;
            this.isCallCorrect = isCallCorrect;

            if (orderValue_opt.HasValue) {
                this.orderValue = (int)orderValue_opt;
            } else {
                this.orderValue = RandomOrderValue(orderType);
            }
        }

        public void Update()
        {
            statusList[^1].time += Time.deltaTime;
        }

        public void ChangeStatus(OrderStatus newStatus)
        {
            statusList.Add(new StatusWithTime(newStatus, 0.0f));
        }

        static public int RandomOrderValue(OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Soul => RandomSoulValue(),
                OrderType.Money => RandomMoneyValue(),
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
        }

        static int RandomSoulValue() 
        {
            return UnityEngine.Random.Range(0, 10);
        }

        static int RandomMoneyValue() 
        {
            return UnityEngine.Random.Range(0, 1000 / 100) * 100;
        }

        override public string ToString()
        {
            var res = string.Format(
                "id: {0}, line: {1}, type: {2}, value: {3}, correct: {4}, result: {5}, rating: {6}\n", 
                id, phoneLine, orderType, orderValue, isCallCorrect, resultQuality, rating);
            
            foreach (var statusWithTime in statusList) {
                res += string.Format("\t- {0}: {1}\n", statusWithTime.status, statusWithTime.time.ToString("0.0"));
            }
            return res.Remove(res.Length - 1);
        }
    }

    private List<Order> orderList;

    public float minTime;
    public float midTime;
    public float maxTime;
    public float curvature;
    public float baseline;

    public float correctCallProbability;

    public float reduceWaitingTimeAfterFrozen;
    private float waitingTimeLeft;
    public bool acceptOrders = true;
    private bool wasFrosen = false;

    private int nextId = 0;
    public int numPhoneLines;

    float RandomTimeToNextOrder()
    {
        return RandomUtils.TruncatedGaussian(minTime, maxTime, midTime, curvature, baseline);
    }

    Order.OrderType RandomOrderType()
    {
        int numTypes = Enum.GetNames(typeof(Order.OrderType)).Length;
        float result = UnityEngine.Random.Range(0, numTypes);
        return (Order.OrderType)result;
    }

    bool RandomIsCallCorrect()
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < correctCallProbability) {
            return true;
        }
        return false;
    }

    private void TestGenerator() 
    {
        using (StreamWriter outputFile = new StreamWriter(Path.Combine("probabilities", "time_values.txt")))
        {
            for (int i = 0; i < 1000000; ++i) {
                outputFile.WriteLine(RandomTimeToNextOrder().ToString());
            }   
        }
    }

    int RandomPhoneLine(List<Order> callingOrders) 
    {
        bool[] isLineBusy = new bool[numPhoneLines];
        int busyLines = 0;
        foreach (var order in callingOrders) {
            isLineBusy[order.phoneLine] = true;
            busyLines += 1;
        }

        int relativeLine = UnityEngine.Random.Range(0, numPhoneLines - busyLines);
        for (int i = 0; i < numPhoneLines; ++i) {
            if (!isLineBusy[i]) {
                if (relativeLine == 0) {
                    return i;
                }
                relativeLine -= 1;
            }
        }
        throw new Exception("No empty lines!");
    }

    List<Order> GetCallingOrders() 
    {
        var orders = new List<Order>();
        foreach(var order in 
                from order in orderList
			    where order.statusList[^1].status == Order.OrderStatus.Calling
                select order) {
            orders.Add(order);
        }
        return orders;
    }

    void Start()
    {
        if (Debug.isDebugBuild) {
            TestGenerator();
        }
        orderList = new List<Order>();
    }

    void Update()
    {
        foreach (var order in orderList)
        {
            order.Update();
        }

        var callingOrders = GetCallingOrders();

        if (acceptOrders) {
            if (callingOrders.Count() < numPhoneLines) {
                waitingTimeLeft -= Time.deltaTime;
                if (wasFrosen) {
                    waitingTimeLeft -= reduceWaitingTimeAfterFrozen;
                    wasFrosen = false;
                }
                if (waitingTimeLeft < 0) {
                    orderList.Add(new Order(nextId++, RandomPhoneLine(callingOrders), RandomOrderType(), RandomIsCallCorrect(), null));
                    waitingTimeLeft = RandomTimeToNextOrder();
                }
            } else {
                wasFrosen = true;
            }
        }
    }

    void OnGUI()
    {
        int movePos(ref int pos, int step)
        {
            pos += step;
            return pos;
        }
        
        // if (Selection.activeGameObject != gameObject)
        // {
        //     return;
        // }
        var textStyle = new GUIStyle();
        textStyle.fontSize = 15;
        int step = (int)(textStyle.fontSize * 1.1);
        int pos = 5;
        var resString = string.Format("Time till next call {0}\nOrder List:", waitingTimeLeft.ToString("0.0"));
        EditorGUI.LabelField(new Rect(10, pos, 150, movePos(ref pos, step * resString.Split('\n').Length)), resString, textStyle);
        foreach (var order in orderList)
        {
            EditorGUI.LabelField(new Rect(10, pos, 350, movePos(ref pos, step * order.ToString().Split('\n').Length)), order.ToString(), textStyle);
        }
    }
}
