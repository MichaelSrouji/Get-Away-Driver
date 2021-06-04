using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class Astar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Arguments needed:
    currentNode;
    endNode;

    Map<Point, Node> openMap = new HashMap<Point, Node>();
    Map<Point, Node> closedMap = new HashMap<Point, Node>();
    PriorityQueue<Node> pQueue = new PriorityQueue<Node>(compF) ;
    List path = new ArrayList();
    Node current = new Node(start, 0, ((Math.abs(start.x - end.x))+(Math.abs(start.y - end.y))) ,null);
    openMap.put(start, current);

    while (pQueue.size() != 0) {
            current = pQueue.poll();
            List<Point> neighbors = potentialNeighbors.apply(current.point).filter(canPassThrough).collect(Collectors.toList());

            for (Point neighbor : neighbors) {

                if (openMap.get(neighbor) == null) {
                    Node neighborNode = new Node(neighbor, current.g += 1, ((Math.abs(neighbor.x - end.x)) + (Math.abs(neighbor.y - end.y))), current);
                    openMap.put(neighbor, neighborNode);
                    pQueue.add(neighborNode);

                }
                closedMap.put(current.point, current);
            }

            if(withinReach.test(current.point, end))
            {
                break;
            }
        }
        
    if (pQueue.size() == 0)
        {
            return path;
        }

    while (current.previous != null) {
            path.add(0, current.point);
            current = current.previous;
        }
    System.out.println("Path " + path);

    return path;
}*/
