using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinningDiamondCollected : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Animator anim;

    [SerializeField] private float timeForDeath = 2;

    [SerializeField] private float startSpinSpeed = 1.5f;

    [SerializeField] private float moveUpSpeed = 1;


    [SerializeField] private GameObject SpawnBurst;
    void Start()
    {
        anim.speed = startSpinSpeed;

        StartCoroutine(waitForDestroy());
    }

    IEnumerator waitForDestroy()
    {
        yield return new WaitForSeconds(timeForDeath);

        Instantiate(SpawnBurst, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        anim.speed += Time.deltaTime;

        moveUpSpeed += Time.deltaTime;

        transform.Translate(Vector2.up * moveUpSpeed * Time.deltaTime);
    }
}
