using UnityEngine;
using System.Collections;



public class AnimationController : MonoBehaviour {
    [SerializeField] Transform particleEffect;
    [SerializeField] functionManager SS;

    public void explode(Vector2 pos, Quaternion rot)
    {
        GameObject a=GameObject.Instantiate(particleEffect.gameObject, SS.hexPositionTransform(pos), rot) as GameObject;
    }
}
