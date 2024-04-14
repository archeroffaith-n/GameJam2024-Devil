using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PhoneInteraction : MonoBehaviour
{
    private class AxisDownUpHandler
    {
        private Dictionary<string, float> value;
        private Dictionary<string, bool> isAxisDown;
        private Dictionary<string, bool> isAxisUp;

        string[] axes;

        public AxisDownUpHandler(string[] axes)
        {
            this.axes = axes;
            value = new Dictionary<string, float>();
            isAxisDown = new Dictionary<string, bool>();
            isAxisUp = new Dictionary<string, bool>();

            foreach (string axis in axes) {
                value.Add(axis, 0.0f);
                isAxisDown.Add(axis, false);
                isAxisUp.Add(axis, false);
            }

            Update();
        }

        public void Update()
        {
            foreach (string axis in axes) {
                float newValue = Input.GetAxis(axis);
                if ((newValue > 0.0f) && (value[axis] == 0.0f)) {
                    isAxisDown[axis] = true;
                } else {
                    isAxisDown[axis] = false;
                }
                if ((newValue == 0.0f) && (value[axis] >= 0.0f)) {
                    isAxisUp[axis] = true;
                } else {
                    isAxisUp[axis] = false;
                }
                value[axis] = newValue;
            }
        }

        public bool GetAxisDown(string axis)
        {
            return isAxisDown[axis];
        }

        public bool GetAxisUp(string axis)
        {
            return isAxisUp[axis];
        }
    }

    HashSet<Phone> availablePhones;
    Phone availablePhone;
    AxisDownUpHandler axisDownUpHandler;

    private bool? couldMove;

    void Start()
    {
        availablePhones = new HashSet<Phone>();
        string[] axes = {"Submit", "Cancel"};
        axisDownUpHandler = new AxisDownUpHandler(axes);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Phone")) {
            return;
        }
        availablePhones.Add(other.gameObject.GetComponent<Phone>());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Phone")) {
            return;
        }
        availablePhones.Remove(other.gameObject.GetComponent<Phone>());
    }

    void UpdateClosestPhone()
    {
        float minDistance = 1000000.0f;
        foreach (var phone in availablePhones) {
            float distance = (phone.transform.position - transform.position).magnitude;
            if (distance < minDistance) {
                availablePhone = phone;
                minDistance = distance;
            }
        }
    }

    void Update()
    {
        axisDownUpHandler.Update();
        UpdateClosestPhone();

        if ((gameObject.GetComponent<LevelsManager>().mode != LevelsManager.Mode.Stop) && !couldMove.HasValue) {
            couldMove = gameObject.GetComponent<PlayerMovement>().canMove;
            gameObject.GetComponent<PlayerMovement>().canMove = false;
        }

        if ((gameObject.GetComponent<LevelsManager>().mode == LevelsManager.Mode.Stop) && couldMove.HasValue) {
            gameObject.GetComponent<PlayerMovement>().canMove = (bool)couldMove;
            couldMove = null;
        }

        if (gameObject.GetComponent<LevelsManager>().mode == LevelsManager.Mode.Stop) {
            if ((availablePhone != null) && (availablePhone.order != null) && (availablePhone.order.currentStatus.status == OrdersManager.Order.OrderStatus.Calling)) {
                if (axisDownUpHandler.GetAxisDown("Submit")) {
                    availablePhone.order.ChangeStatus(OrdersManager.Order.OrderStatus.Answered);
                    gameObject.GetComponent<PlayerMovement>().canMove = false;
                    gameObject.GetComponent<LevelsManager>().SplitLevel(availablePhone.order.levelAndSpell.level);
                }
            } else if ((availablePhone != null) && (availablePhone.order != null) && (availablePhone.order.currentStatus.status == OrdersManager.Order.OrderStatus.Answered)) {
                if (axisDownUpHandler.GetAxisDown("Submit")) {
                    availablePhone.order.ChangeStatus(OrdersManager.Order.OrderStatus.Accepted);
                    gameObject.GetComponent<PlayerMovement>().canMove = true;
                    gameObject.GetComponent<LevelsManager>().UnsplitLevel(true);
                } else if (axisDownUpHandler.GetAxisDown("Cancel")) {
                    availablePhone.order.ChangeStatus(OrdersManager.Order.OrderStatus.Aborted);
                    gameObject.GetComponent<PlayerMovement>().canMove = true;
                    gameObject.GetComponent<LevelsManager>().UnsplitLevel(false);
                }
            }    
        }    
    }
}
