using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace WarriorAnimsFREE
{
	public class GUIControls:MonoBehaviour
	{
		private WarriorController warriorController;
		[SerializeField] GameManager gameManager;
		[SerializeField] UnityEvent criticalDamage;

		private void Awake()
		{
			warriorController = GetComponent<WarriorController>();

		}

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
			{
                criticalDamage.Invoke();
            }
        }

        private void OnGUI()
		{
			if (warriorController.canAction) {
				Attacking();
				Jumping();
				CriticalDamage();

            }

			Debug();
		}

		private void Attacking()
		{
			if (warriorController.MaintainingGround() && warriorController.canAction) {
					if (GUI.Button(new Rect(25, 85, 100, 30), "Attack1")) { warriorController.Attack1(); }
			}
		}

		private void CriticalDamage()
		{
			if (GUI.Button(new Rect(25, 135, 100, 30), "Critical")) { warriorController.takeExplodeDamage = true; warriorController.CriticalDamage(); }
		}

		private void Jumping()
		{
			if (warriorController.canJump
				&& warriorController.canAction) {
                if (warriorController.MaintainingGround()) {
                    if (GUI.Button(new Rect(25, 175, 100, 30), "Jump")) {
						if (warriorController.canJump) { warriorController.inputJump = true; UnityEngine.Debug.Log("ジャンプは押したぞ"); ; }
					}
				}
			}
		}

		private void Debug()
		{
			if (GUI.Button(new Rect(600, 15, 120, 30), "Debug Controller")) { warriorController.ControllerDebug(); }
			if (GUI.Button(new Rect(600, 50, 120, 30), "Debug Animator")) { warriorController.AnimatorDebug(); }
		}
	}
}