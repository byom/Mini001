using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

public class Board : MonoBehaviour
{
    public enum State
    {
        WAIT, PROCESSING, END
    }

    public State state = State.WAIT;
    public static Board Instance
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<Board>();
            return _inst;
        }
    }
    private static Board _inst;
    public List<NodeObject> realNodeList = new List<NodeObject>();
    public List<Node> nodeData = new List<Node>();
    public Dictionary<Vector2Int, Node> nodeMap = new Dictionary<Vector2Int, Node>();
    public int col = 4;
    public int row = 4;
    public GameObject emptyNodePrefab;
    public GameObject nodePrefab;
    public RectTransform emptyNodeRect;
    public RectTransform realNodeRect;

    public UnityAction<int> OnUpdateScoreAction;

    private Direction dir = Direction.NULL;
    private Vector2 TouchPosition;

    public void OnGameOver()
    {
        Debug.Log("Game Over!!!!");
    }

    public int GetScore()
    {
        int score = 0;
        for (int i = col - 1; i >= 0; i--)
        {
            for (int j = 0; j < row; j++)
            {
                var p = nodeMap[new Vector2Int(j, i)].value;
                if (p.HasValue)
                {
                    score += p.Value;
                }
            }
        }

        return score;
    }


    public void ResetGame()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        /* first initialize empty Node rect*/
        realNodeList.Clear();
        nodeMap.Clear();
        nodeData.Clear();
        var emptyChildCount = emptyNodeRect.transform.childCount;
        for (int i = 0; i < emptyChildCount; i++)
        {
            var child = emptyNodeRect.GetChild(i);
        }

        /* and, empty node create for get grid point*/
        for (int i = 0; i < col; i++)
        {
            for (int j = 0; j < row; j++)
            {
                var instantiatePrefab = GameObject.Instantiate(emptyNodePrefab, emptyNodeRect.transform, false);
                var point = new Vector2Int(j, i);
                //r-d-l-u
                Vector2Int left = point - new Vector2Int(1, 0);
                Vector2Int down = point - new Vector2Int(0, 1);
                Vector2Int right = point + new Vector2Int(1, 0);
                Vector2Int up = point + new Vector2Int(0, 1);
                Vector2Int?[] v = new Vector2Int?[4];
                if (IsValid(right)) v[0] = right;
                if (IsValid(down)) v[1] = down;
                if (IsValid(left)) v[2] = left;
                if (IsValid(up)) v[3] = up;
                Node node = new Node(v);
                node.point = point;
                node.nodeRectObj = instantiatePrefab;
                nodeData.Add(node);
                instantiatePrefab.name = node.point.ToString();
                this.nodeMap.Add(point, node);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(emptyNodeRect);
        foreach (var data in nodeData)
            data.position = data.nodeRectObj.GetComponent<RectTransform>().localPosition;
    }

    private bool IsValid(Vector2Int point)
    {
        if (point.x == -1 || point.x == row || point.y == col || point.y == -1)
            return false;

        return true;
    }
    private void CreateBlock(int x, int y)
    {
        if (nodeMap[new Vector2Int(x, y)].realNodeObj != null) return;

        GameObject realNodeObj = Instantiate(nodePrefab, realNodeRect.transform, false);
        var node = nodeMap[new Vector2Int(x, y)];
        var pos = node.position;
        realNodeObj.GetComponent<RectTransform>().localPosition = pos;
        realNodeObj.transform.DOPunchScale(new Vector3(.32f, .32f, .32f), 0.15f, 3);
        var nodeObj = realNodeObj.GetComponent<NodeObject>();
        this.realNodeList.Add(nodeObj);
        nodeObj.InitializeFirstValue();
        node.value = nodeObj.value;
        node.realNodeObj = nodeObj;
    }

    public void Combine(Node from, Node to)
    {
        to.value = to.value * 2;
        from.value = null;
        if (from.realNodeObj != null)
        {
            from.realNodeObj.CombineToNode(from, to);
            from.realNodeObj = null;
            to.combined = true;
        }
    }

    public void Move(Node from, Node to)
    {
        to.value = from.value;
        from.value = null;
        if (from.realNodeObj != null)
        {
            from.realNodeObj.MoveToNode(from, to);
            if (from.realNodeObj != null)
            {
                to.realNodeObj = from.realNodeObj;
                from.realNodeObj = null;
                Debug.Log(to.realNodeObj != null);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dir"></param>
    public void MoveTo(Direction dir)
    {
        Debug.LogError("Move dir:" + dir.ToString());
        if (dir == Direction.RIGHT)
        {
            for (int j = 0; j < col; j++)
            {
                for (int i = (row - 2); i >= 0; i--)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;
                    var right = node.FindTarget(node, Direction.RIGHT);
                    if (right != null)
                    {
                        if (node.value.HasValue && right.value.HasValue)
                        {
                            if (node.value == right.value)
                            {
                                Combine(node, right);
                            }
                        }
                        else if (right != null && right.value.HasValue == false)
                        {
                            Move(node, right);
                        }
                        else if (right == null)
                            return;
                    }
                }
            }

        }
        if (dir == Direction.LEFT)
        {
            for (int j = 0; j < col; j++)
            {
                for (int i = 1; i < row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;

                    var left = node.FindTarget(node, Direction.LEFT);
                    if (left != null)
                    {
                        if (node.value.HasValue && left.value.HasValue)
                        {
                            if (node.value == left.value)
                            {
                                Combine(node, left);
                            }
                        }
                        else if (left != null && left.value.HasValue == false)
                        {
                            Move(node, left);
                        }
                    }
                }
            }

        }
        if (dir == Direction.UP)
        {
            for (int j = col - 2; j >= 0; j--)
            {
                for (int i = 0; i < row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;
                    var up = node.FindTarget(node, Direction.UP);
                    if (up != null)
                    {
                        if (node.value.HasValue && up.value.HasValue)
                        {
                            if (node.value == up.value)
                            {
                                Combine(node, up);
                            }
                        }
                        else if (up != null && up.value.HasValue == false)
                        {
                            Move(node, up);
                        }
                    }
                }
            }
        }
        if (dir == Direction.DOWN)
        {
            for (int j = 1; j < col; j++)
            {
                for (int i = 0; i < row; i++)
                {
                    var node = nodeMap[new Vector2Int(i, j)];
                    if (node.value == null)
                        continue;
                    var down = node.FindTarget(node, Direction.DOWN);
                    if (down != null)
                    {
                        if (node.value.HasValue && down.value.HasValue)
                        {
                            if (node.value == down.value)
                            {
                                Combine(node, down);
                            }
                        }
                        else if (down != null && down.value.HasValue == false)
                        {
                            Move(node, down);
                        }
                    }
                }
            }
        }

        foreach (var data in realNodeList)
        {
            if (data.target != null)
            {
                state = State.PROCESSING;
                data.StartMoveAnimation();
            }
        }

        // Show();
        if (IsGameOver())
        {
            OnGameOver();
        }
    }

    /// <summary>
    /// if can't combine anymore then game over!!!!
    /// </summary>
    /// <returns></returns>
    public bool IsGameOver()
    {
        bool gameOver = true;
        nodeData.ForEach(x =>
        {
            for (int i = 0; i < x.linkedNode.Length; i++)
            {
                if (x.realNodeObj == null) gameOver = false;
                if (x.linkedNode[i] == null)
                    continue;

                var nearNode = nodeMap[x.linkedNode[i].Value];
                if (x.value == null || nearNode.value == null)
                    continue;

                if (x.value == nearNode.value)
                {
                    gameOver = false;
                }
            }
        });

        return gameOver;
    }
    private void CreateRandom()
    {
        var emptys = nodeData.FindAll(x => x.realNodeObj == null);
        if (emptys.Count == 0)
        {
            if (IsGameOver())
            {
                OnGameOver(); ;
            }
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, emptys.Count);
            var pt = emptys[rand].point;
            CreateBlock(pt.x, pt.y);
        }
    }
    private void Awake()
    {
        CreateBoard();
    }

    public void UpdateState()
    {
        bool targetAllNull = true;
        foreach (var data in realNodeList)
        {
            if (data.target != null)
            {
                targetAllNull = false;
                break;
            }
        }

        if (targetAllNull)
        {
            if (state == State.PROCESSING)
            {
                var removed = new List<NodeObject>();
                List<NodeObject> removeTarget = new List<NodeObject>();
                foreach (var data in realNodeList)
                    if (data.needDestroy)
                        removeTarget.Add(data);

                removeTarget.ForEach(x =>
                {
                    realNodeList.Remove(x);
                    GameObject.Destroy(x.gameObject);
                });
                state = State.END;
            }
        }

        if (state == State.END)
        {
            nodeData.ForEach(x => x.combined = false);

            state = State.WAIT;
            CreateRandom();
            CreateRandom();
        }
    }

    private void Show()
    {
        string v = null;
        for (int i = col - 1; i >= 0; i--)
        {
            for (int j = 0; j < row; j++)
            {
                var p = nodeMap[new Vector2Int(j, i)].value;
                string t = p.ToString();
                if (p.HasValue == false)
                {
                    t = "N";
                }
                if (p == 0) t = "0";

                v += t + " ";
            }
            v += "\n";
        }
        Debug.Log(v);
    }

    private void UpdateScore()
    {
        var score = GetScore();
        if (OnUpdateScoreAction != null)
        {
            OnUpdateScoreAction(score);
        }
    }

    private void Update()
    {
        UpdateState();

        if (state == State.WAIT)
        {

            var direction = Direction.NULL;
#if UNITY_EDITOR || UNITY_STANDALONE
            direction = UpdateKeyboard();
#else
            direction = UpdateGesture();
#endif

            if (direction != Direction.NULL)
                MoveTo(direction);
        }

        UpdateScore();
    }

    private void Start()
    {
        CreateRandom();
        CreateRandom();
    }

    Direction UpdateKeyboard()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
            return (Direction.RIGHT);
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            return (Direction.LEFT);
        if (Input.GetKeyUp(KeyCode.UpArrow))
            return (Direction.UP);
        if (Input.GetKeyUp(KeyCode.DownArrow))
            return (Direction.DOWN);

        return Direction.NULL;
    }

    Direction UpdateGesture()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TouchPosition = Input.GetTouch(0).position;
        }

        if (Input.GetTouch(0).phase != TouchPhase.Ended)
            return Direction.NULL;

        Direction dir = CheckDir();
        return dir;
    }

    private Direction CheckDir()
    {

        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {

            Vector2 offest = Input.GetTouch(0).position - TouchPosition;


            if (Mathf.Abs(offest.x) > 100 && Mathf.Abs(offest.x) >= Mathf.Abs(offest.y))
            {
                if (offest.x < 0)
                    dir = Direction.LEFT;
                else
                    dir = Direction.RIGHT;
            }

            if (Mathf.Abs(offest.y) > 100 && Mathf.Abs(offest.x) < Mathf.Abs(offest.y))
            {
                if (offest.y > 0)
                    dir = Direction.UP;
                else
                    dir = Direction.DOWN;
            }

            if (Mathf.Abs(offest.x) <= 100 && Mathf.Abs(offest.y) <= 100)
                dir = Direction.NULL;
        }
        return dir;
    }
}


