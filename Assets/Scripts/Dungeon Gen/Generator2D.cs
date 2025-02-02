using System.Collections.Generic;
using Graphs;
using Unity.AI.Navigation;
using UnityEngine;
using Random = System.Random;

public class Generator2D : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        Door,
        Hallway
    }

    class Room
    {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [Header("Configuration")]
    [SerializeField]
    private MapGeneratorConfig config;

    [Header("Debug variables")]
    [Space(1)]
    [SerializeField]
    bool debug = true;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material greenMaterial;
    [SerializeField]
    Material purpleMaterial;

    GameObject mapObject;
    Transform parentTransform;
    Transform floorTransform;
    Transform ceilingTransform;
    Transform lightsTransform;

    Random random;
    Grid2D<CellType> grid;
    List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;
    Room smallestRoom;

    void Start()
    {
        // Create the empty GameObject with the name "map_seedNum"
        mapObject = new GameObject("map_" + config.randomSeed);

        parentTransform = mapObject.transform;
        //parentTransform.localScale = new Vector3(mapMultiplier, mapMultiplier, mapMultiplier);

        // Crear y configurar el Transform de Floors
        GameObject floorObject = new GameObject("Floors");
        floorTransform = floorObject.transform;
        floorTransform.parent = parentTransform;
        floorTransform.localPosition = Vector3.zero;

        // Crear y configurar el Transform de Ceilings
        GameObject ceilingObject = new GameObject("Ceilings");
        ceilingTransform = ceilingObject.transform;
        ceilingTransform.parent = parentTransform;
        ceilingTransform.localPosition = Vector3.zero;

        // Crear y configurar el Transform de Lights
        GameObject lightsObject = new GameObject("Lights");
        lightsTransform = lightsObject.transform;
        lightsTransform.parent = parentTransform;
        lightsTransform.localPosition = Vector3.zero;


        // Optionally, you can call a method to generate the map (if needed)
        Generate();
    }

    void Generate()
    {
        random = new Random(config.randomSeed);
        grid = new Grid2D<CellType>(config.size, Vector2Int.zero);
        rooms = new List<Room>();
        PlaceRooms();
        Triangulate();
        CreateHallways();

        grid.Print();

        PathfindHallways();

        //debug
        DrawSpawnRoom();

        grid.Print();

        //Parte visible
        BuildLevel();

        //PlaceLights();

        PlacePlayer();

        parentTransform.localScale = new Vector3(config.mapMultiplier, config.mapMultiplier, config.mapMultiplier);

        PlaceNavMesh();
    }

    private void PlaceNavMesh()
    {
        // Add a NavMeshSurface component to this GameObject
        NavMeshSurface navMeshSurface = mapObject.AddComponent<NavMeshSurface>();

        navMeshSurface.layerMask = LayerMask.GetMask("Floor", "Wall");

        // Bake the NavMesh for this surface
        navMeshSurface.BuildNavMesh();
    }

    void PlaceRooms()
    {
        // Crear la primera sala con tamaño mínimo
        bool minRoomPlaced = false;
        while (!minRoomPlaced)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, config.size.x),
                random.Next(0, config.size.y)
            );

            Vector2Int roomSize = config.roomMinSize; // Forzamos el tamaño mínimo.
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            bool add = true;

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= config.size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= config.size.y)
            {
                add = false;
            }

            if (add)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }

                smallestRoom = newRoom; // Guardamos esta sala.
                minRoomPlaced = true; // Confirmamos que la sala mínima ha sido colocada.
            }
        }

        // Continuar generando salas restantes aleatoriamente
        for (int i = 0; i < config.roomCount; i++)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, config.size.x),
                random.Next(0, config.size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                random.Next(config.roomMinSize.x, config.roomMaxSize.x + 1),
                random.Next(config.roomMinSize.y, config.roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= config.size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= config.size.y)
            {
                add = false;
            }

            if (add)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (random.NextDouble() < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways()
    {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(config.size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) =>
            {
                var pathCost = new DungeonPathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost += 10;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost += 5;
                }
                else if (grid[b.Position] == CellType.Hallway)
                {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

                for (int i = 0; i < path.Count - 1; i++)
                {
                    var curr = path[i];
                    var next = path[i + 1];

                    if (grid[curr] == CellType.Hallway)
                    {
                        PlaceHallway(curr);
                    }

                    // Verificar si la posición actual es una sala y la siguiente es un pasillo
                    if (grid[curr] == CellType.Room && grid[next] == CellType.Hallway)
                    {
                        // Verificar que la ubicación anterior no es una puerta antes de colocarla
                        // if (grid[next] != CellType.Door)
                        {
                            PlaceDoor(curr);
                            grid[curr] = CellType.Door;  // Asignar 'Door' en la posición donde se coloca la puerta
                        }
                    }
                    else if (grid[next] == CellType.Room && grid[curr] == CellType.Hallway)
                    {
                        // Verificar que la ubicación anterior no es una puerta antes de colocarla
                        // if (grid[next] != CellType.Door)
                        {
                            PlaceDoor(next);
                            grid[next] = CellType.Door;  // Asignar 'Door' en la posición donde se coloca la puerta
                        }
                    }
                }
            }
        }
    }

    private void PlaceDoors()
    {
        // Recorrer el grid completo
        for (int y = 0; y < config.size.y; y++)
        {
            for (int x = 0; x < config.size.x; x++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);

                // Verificar si la celda actual es un pasillo
                if (grid[currentPos] == CellType.Hallway)
                {
                    // Verificar si algún vecino es una habitación
                    foreach (var neighbor in GetNeighbors(currentPos))
                    {
                        // Si el vecino es una habitación, colocar una puerta en la posición actual
                        if (grid[neighbor] == CellType.Room)
                        {
                            PlaceDoor(currentPos);
                            grid[currentPos] = CellType.Door;
                            break; // Solo colocar una puerta una vez por pasillo
                        }
                    }
                }
            }
        }
    }

    private List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Definir las posiciones de los vecinos (arriba, abajo, izquierda, derecha)
        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, 1),  // Arriba
        new Vector2Int(0, -1), // Abajo
        new Vector2Int(1, 0),  // Derecha
        new Vector2Int(-1, 0)  // Izquierda
        };

        // Comprobar los vecinos en las direcciones definidas
        foreach (var direction in directions)
        {
            Vector2Int neighbor = pos + direction;
            if (InBounds(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private bool InBounds(Vector2Int pos)
    {
        // Verifica si la posición está dentro de los límites del grid
        return pos.x >= 0 && pos.x < config.size.x && pos.y >= 0 && pos.y < config.size.y;
    }

    void BuildLevel()
    {
        for (int y = 0; y < config.size.y; y++)
        {
            for (int x = 0; x < config.size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (grid[pos] == CellType.Room || grid[pos] == CellType.Hallway)
                {
                    Instantiate(config.floorTilePrefab, new Vector3(x, 0, y), Quaternion.identity, floorTransform);

                    // Si generateCeilling está activo, generar un techo
                    if (config.generateCeiling)
                    {
                        // Crear el techo con orientación hacia abajo
                        Instantiate(config.ceilingTilePrefab, new Vector3(x, 1.5f, y), Quaternion.Euler(180, 0, 0), ceilingTransform);
                    }
                }
                else if (grid[pos] == CellType.Door)
                {
                    Instantiate(config.floorTilePrefab, new Vector3(x, 0, y), Quaternion.identity, floorTransform);

                    // Si generateCeilling está activo, generar un techo
                    if (config.generateCeiling)
                    {
                        Instantiate(config.ceilingTilePrefab, new Vector3(x, 1.5f, y), Quaternion.Euler(180, 0, 0), ceilingTransform);
                    }

                    PlaceDoorWithOrientation(pos);
                }
            }
        }

        // Colocar paredes en un segundo paso
        for (int y = 0; y < config.size.y; y++)
        {
            for (int x = 0; x < config.size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                PlaceWalls(pos);
                // Colocar pilares después de las paredes
                PlacePillars(pos);
            }
        }
    }

    void PlaceDoorWithOrientation(Vector2Int pos)
    {
        Vector2Int? roomNeighbor = null;
        Vector2Int? hallwayNeighbor = null;

        // Buscar vecinos Room y Hallway
        foreach (var neighbor in GetNeighbors(pos))
        {
            if (grid[neighbor] == CellType.Room)
            {
                roomNeighbor = neighbor;
            }
            else if (grid[neighbor] == CellType.Hallway)
            {
                hallwayNeighbor = neighbor;
            }
        }

        // Verifica si hay conexión válida entre Room y Hallway
        if (roomNeighbor.HasValue && hallwayNeighbor.HasValue)
        {
            // Calcula la dirección hacia la habitación
            Vector3 direction = new Vector3(hallwayNeighbor.Value.x - pos.x, 0, hallwayNeighbor.Value.y - pos.y);
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

            // Ajusta la posición de la puerta para que esté en el borde
            Vector3 doorPosition = new Vector3(pos.x + direction.x * 0.5f, 0, pos.y + direction.z * 0.5f);

            // Instancia la puerta
            Instantiate(config.doorPrefab, doorPosition, rotation, parentTransform);
        }
        else
        {
            // Si no hay una conexión válida, coloca la puerta sin rotación como fallback
            Instantiate(config.doorPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity, parentTransform);
        }
    }

    void PlaceWalls(Vector2Int pos)
    {
        bool isAtMapEdge = pos.x == 0 || pos.x == config.size.x || pos.y == 0 || pos.y == config.size.y;

        if (grid[pos] == CellType.None)
        {
            // Coloca la pared orientada hacia la celda ocupada más cercana
            foreach (var neighbor in GetNeighbors(pos))
            {
                if (grid[neighbor] != CellType.None)
                {
                    Vector3 direction = new Vector3(neighbor.x - pos.x, 0, neighbor.y - pos.y);
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                    Instantiate(config.wallPrefab, new Vector3(pos.x + direction.x * 0.5f, 0, pos.y + direction.z * 0.5f), rotation, parentTransform);

                    if (pos.x % 2 == 0)
                        Instantiate(config.lampPrefab, new Vector3(pos.x + direction.x * 0.5f, 1.25f, pos.y + direction.z * 0.5f), rotation, lightsTransform);
                    else if (pos.y % 2 == 0)
                        Instantiate(config.lampPrefab, new Vector3(pos.x + direction.x * 0.5f, 1.25f, pos.y + direction.z * 0.5f), rotation, lightsTransform);
                }
            }
            return;
        }

        if (isAtMapEdge && grid[pos] != CellType.None)
        {
            // Determina hacia qué lado del mapa está el borde y coloca la pared en la dirección opuesta
            Vector3 direction = Vector3.zero;

            // Determina la dirección de la pared según la posición en los bordes
            if (pos.x == 0) // Borde izquierdo
            {
                direction = Vector3.left;
            }
            else if (pos.x == config.size.x - 1) // Borde derecho
            {
                direction = Vector3.right;
            }
            else if (pos.y == 0) // Borde inferior
            {
                direction = Vector3.back;
            }
            else if (pos.y == config.size.y - 1) // Borde superior
            {
                direction = Vector3.forward;
            }

            // Coloca la pared con la dirección opuesta al borde
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            Instantiate(config.wallPrefab, new Vector3(pos.x + direction.x * 0.5f, 0, pos.y + direction.z * 0.5f), rotation, parentTransform);

            if (pos.x % 2 == 0)
                Instantiate(config.lampPrefab, new Vector3(pos.x + direction.x * 0.5f, 1.25f, pos.y + direction.z * 0.5f), rotation, lightsTransform);
            else if (pos.y % 2 == 0)
                Instantiate(config.lampPrefab, new Vector3(pos.x + direction.x * 0.5f, 1.25f, pos.y + direction.z * 0.5f), rotation, lightsTransform);

            return;
        }

        if (grid[pos] == CellType.Hallway)
        {

            // Coloca la pared orientada hacia la celda ocupada más cercana
            foreach (var neighbor in GetNeighbors(pos))
            {
                if (grid[neighbor] == CellType.Room)
                {
                    Vector3 direction = new Vector3(neighbor.x - pos.x, 0, neighbor.y - pos.y);
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                    Instantiate(config.wallPrefab, new Vector3(pos.x + direction.x * 0.5f, 0, pos.y + direction.z * 0.5f), rotation, parentTransform);

                    if (pos.x % 2 == 0)
                        Instantiate(config.lampPrefab, new Vector3(pos.x + direction.x * 0.5f, 1.25f, pos.y + direction.z * 0.5f), rotation, lightsTransform);
                    else if (pos.y % 2 == 0)
                        Instantiate(config.lampPrefab, new Vector3(pos.x + direction.x * 0.5f, 1.25f, pos.y + direction.z * 0.5f), rotation, lightsTransform);
                }
            }
            return;
        }
    }
    void PlaceHallwayWalls(Vector2Int pos)
    {
        if (grid[pos] == CellType.Hallway)
        {
            foreach (var neighbor in GetNeighbors(pos))
            {
                if (grid[neighbor] == CellType.None)
                {
                    // Coloca una pared en celdas vacías adyacentes al pasillo
                    Vector3 direction = new Vector3(pos.x - neighbor.x, 0, pos.y - neighbor.y);
                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                    Instantiate(config.wallPrefab, new Vector3(neighbor.x, 0, neighbor.y), rotation, parentTransform);
                }
            }
        }
    }
    void PlacePillars(Vector2Int pos)
    {
        if (grid[pos] == CellType.Room)
        {
            int adjacentCount = 0;
            List<Vector2Int> neighborsWithWalls = new List<Vector2Int>();

            foreach (var neighbor in GetNeighbors(pos))
            {

                if (grid[neighbor] == CellType.Room)
                {
                    adjacentCount++;
                    neighborsWithWalls.Add(neighbor);
                }
            }
            // Colocar un pilar si es una esquina (dos vecinos perpendiculares)
            if (adjacentCount <= 2)
            {
                if (neighborsWithWalls.Count >= 2)
                {
                    Vector2Int neighbor1 = neighborsWithWalls[0];
                    Vector2Int neighbor2 = neighborsWithWalls[1];

                    // Calculamos las direcciones relativas de los vecinos
                    Vector3 direction1 = new Vector3(neighbor1.x - pos.x, 0, neighbor1.y - pos.y);
                    Vector3 direction2 = new Vector3(neighbor2.x - pos.x, 0, neighbor2.y - pos.y);

                    // Calculamos la dirección del pilar, que será el promedio de las dos direcciones
                    Vector3 pilarDirection = (direction1 + direction2).normalized;

                    // Calculamos la posición intermedia entre los dos vecinos para colocar el pilar
                    Vector3 pillarPos = new Vector3(pos.x - pilarDirection.x * 0.5f, 0, pos.y - pilarDirection.z * 0.5f);

                    // Definimos la rotación del pilar según la dirección
                    Quaternion rotation = Quaternion.LookRotation(pilarDirection, Vector3.up);

                    // Colocamos el pilar en la posición calculada
                    Instantiate(config.pillarPrefab, pillarPos, Quaternion.identity, parentTransform);
                    //Instantiate(pillarPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                }
            }
        }
    }
    private void PlacePlayer()
    {
        // Calcular el centro de la habitación más pequeña
        Vector2Int center = new Vector2Int(
            smallestRoom.bounds.x + smallestRoom.bounds.width / 2 - 1,
            smallestRoom.bounds.y + smallestRoom.bounds.height / 2 - 1
        );

        // Verificar si el prefab del jugador está definido
        if (config.playerPrefab == null)
        {
            Debug.LogWarning("Player prefab is null. Cannot place player.");
            //return;
        }
        else
        {
            // Instanciar al jugador en el centro de la habitación más pequeña
            Instantiate(config.playerPrefab, new Vector3(center.x, 0.5f, center.y) * config.mapMultiplier, Quaternion.identity);
        }

        // Verificar si la habitación más pequeña está definida
        if (smallestRoom == null)
        {
            Debug.LogWarning("Smallest room is not defined. Cannot place player.");
            //return;
        }
        else
        {
            // Instanciamos el objeto
            GameObject instantiatedExitArea = Instantiate(config.exitArea, new Vector3(center.x, 0.5f, center.y) * config.mapMultiplier, Quaternion.identity);
            //instantiatedExitArea.transform.localScale = new Vector3(config.roomMinSize.x, 2, config.roomMinSize.y);
            BoxCollider boxCollider = instantiatedExitArea.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                // Cambia el tamaño del collider
                boxCollider.size = new Vector3(config.roomMinSize.x, 2, config.roomMinSize.y);
            }

            Debug.Log($"Player placed at room center: {center}");
        }
    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material, int locationY = 0)
    {
        if (debug)
        {
            GameObject go = Instantiate(cubePrefab, new Vector3(location.x - 0.5f, locationY, location.y - 0.5f), Quaternion.identity);
            go.transform.SetParent(this.transform);
            go.GetComponent<Transform>().localScale = new Vector3(size.x, 1, size.y);
            go.GetComponent<MeshRenderer>().material = material;
        }
    }

    void PlaceRoom(Vector2Int location, Vector2Int size)
    {
        PlaceCube(location, size, redMaterial);
    }

    void PlaceHallway(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), blueMaterial);
    }

    void PlaceDoor(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), greenMaterial, 2);
    }

    // Método para dibujar el cuadrado púrpura sobre la sala más pequeña
    void DrawSpawnRoom()
    {
        if (smallestRoom != null)
        {
            PlaceCube(smallestRoom.bounds.position, smallestRoom.bounds.size, purpleMaterial, 1);
        }
    }

}
