using UnityEngine;

namespace Code.Interactions
{
 public class TriggerDetect : MonoBehaviour
 {
  public GameObject target;
  void OnTriggerEnter(Collider other)
  {
   target = this.gameObject.transform.GetChild(0).gameObject;
   target.SetActive(true);
   Debug.Log("Object Entered the trigger");
  }

  void OnTriggerStay(Collider other)
  {
   Debug.Log("Object is within trigger");
  }

  void OnTriggerExit(Collider other)
  {
   target = this.gameObject.transform.GetChild(0).gameObject;
   target.SetActive(false);
   Debug.Log ("object Exited the trigger");
  }
 }
}

