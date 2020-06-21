using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    GameControllerScript game;
    public GameObject linePrefab;
    public GameObject currentLine;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    public List<Vector2> drawPositions;
    public Transform drawTopLeft, drawBottomRight;

    public bool isDraw = true;

    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<GameControllerScript>();
        inkText.text = "Ink Remaining: " + inkLimit.ToString();
    }

    bool drawStarted = false;
    public float inkUsed = 0;
    public float inkLimit;
    public Text inkText;
    // Update is called once per frame
    void Update()
    {
        if (isDraw) {
            if (Input.GetMouseButtonDown(0)) {
                Vector2 tempDrawPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (tempDrawPos.x > drawTopLeft.position.x && tempDrawPos.x < drawBottomRight.position.x &&
                    tempDrawPos.y < drawTopLeft.position.y && tempDrawPos.y > drawBottomRight.position.y) {
                    CreateLine();
                }
            }
            if (Input.GetMouseButton(0)) {
                Vector2 tempDrawPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (tempDrawPos.x > drawTopLeft.position.x && tempDrawPos.x < drawBottomRight.position.x &&
                    tempDrawPos.y < drawTopLeft.position.y && tempDrawPos.y > drawBottomRight.position.y) {
                    if (Vector2.Distance(tempDrawPos, drawPositions[drawPositions.Count - 1]) > .1f) {
                        UpdateLineNewPoint(tempDrawPos);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0) && drawStarted) {
                if(!isSpinning)
                    StartCoroutine(LineFinish());
            }
        } else {
            MoveLine();
        }


    }

    void CreateLine() {
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        drawPositions.Clear();
        drawPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        drawPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, drawPositions[0]);
        lineRenderer.SetPosition(1, drawPositions[1]);
        edgeCollider.points = drawPositions.ToArray();
        minX = 0;
        maxX = 0;
        drawStarted = true;
    }


    float minX = 0, maxX = 0;

    void UpdateLineNewPoint(Vector2 newDrawPos) {

        inkUsed += Vector2.Distance(drawPositions[drawPositions.Count - 1], newDrawPos);
        if (inkUsed > inkLimit) {
            if (!isSpinning)
                StartCoroutine(LineFinish());
        } else {
            drawPositions.Add(newDrawPos);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newDrawPos);
            edgeCollider.points = drawPositions.ToArray();
            maxX = Mathf.Max(newDrawPos.x, maxX);
            minX = Mathf.Min(newDrawPos.x, minX);
            inkText.text = "Ink Remaining: " + (Mathf.Round((inkLimit - inkUsed) * 100f) / 100f).ToString();
        }

        
        
    }


    public Transform pivot;
    public void RotateDrawing(float angle) {
        lineRenderer.transform.RotateAround(pivot.transform.position, Vector3.forward, angle);
    }


    float spinAnimTime = 1f;
    int frames = 5;
    bool isSpinning = false;
    [HideInInspector] public bool startGame = false;
    IEnumerator LineFinish() {
        isDraw = false;
        isSpinning = true;
        for(int i = 0; i < frames; i++) {
            RotateDrawing(Random.Range(0, 360));
            yield return new WaitForSecondsRealtime(spinAnimTime / frames);            
        }
        RotateDrawing(Random.Range(0, 360));

        yield return new WaitForSecondsRealtime(1f);
        isSpinning = false;
        Time.timeScale = 1;
        startGame = true;

        yield return new WaitForSecondsRealtime(1f);

        game.SpawnStar();
        game.SpawnHazard();
    }


    public float moveAmount;
    public float xBounds;
    void MoveLine() {
        
        if (Input.GetAxisRaw("Horizontal") > 0 && maxX < xBounds) {
            lineRenderer.transform.position += Vector3.right * moveAmount * Time.deltaTime;
            pivot.transform.position += Vector3.right * moveAmount * Time.deltaTime;
            minX += moveAmount * Time.deltaTime;
            maxX += moveAmount * Time.deltaTime;
        } else if (Input.GetAxisRaw("Horizontal") < 0 && minX > -xBounds) {
            lineRenderer.transform.position += Vector3.left * moveAmount * Time.deltaTime;
            pivot.transform.position += Vector3.left * moveAmount * Time.deltaTime;

            minX -= moveAmount * Time.deltaTime;
            maxX -= moveAmount * Time.deltaTime;
        }
    }

    public void DeleteLine() {
        minX = maxX = inkUsed = 0;
        pivot.transform.position = game.drawZone.transform.position;
        drawStarted = false;
        startGame = false;

        inkText.text = "Ink Remaining: " + inkLimit.ToString();
        Destroy(currentLine);
    }
}
