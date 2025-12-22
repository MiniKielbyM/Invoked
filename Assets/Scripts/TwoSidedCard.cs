using UnityEngine;
public class TwoSidedCard : MonoBehaviour
{
    public GameObject frontSide;
    public GameObject backSide;
    public bool startOnFront = true;
    bool lastFront;
    void Start()
    {
        lastFront = startOnFront;
        frontSide.SetActive(lastFront);
        backSide.SetActive(!lastFront);
    }
    void Update()
    {
        transform.Rotate(0f, Time.deltaTime * 90f, 0f);

        Vector3 f = transform.forward;
        Vector3 c = Camera.main.transform.forward;
        bool isFront = Vector3.Dot(f, c) < 0f;
        if (isFront != lastFront && !startOnFront)
        {
            frontSide.SetActive(isFront);
            backSide.SetActive(!isFront);
            lastFront = isFront;
        }
        else if (isFront != lastFront && startOnFront)
        {
            frontSide.SetActive(!isFront);
            backSide.SetActive(isFront);
            lastFront = isFront;
        }
    }
}
