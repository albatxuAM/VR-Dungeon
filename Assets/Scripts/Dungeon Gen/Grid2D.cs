

using System.Text;
using UnityEngine;

public class Grid2D<T>
{
    T[] data;

    public Vector2Int Size { get; private set; }
    public Vector2Int Offset { get; set; }

    public Grid2D(Vector2Int size, Vector2Int offset)
    {
        Size = size;
        Offset = offset;

        data = new T[size.x * size.y];
    }

    public int GetIndex(Vector2Int pos)
    {
        return pos.x + (Size.x * pos.y);
    }

    public bool InBounds(Vector2Int pos)
    {
        return new RectInt(Vector2Int.zero, Size).Contains(pos + Offset);
    }

    public T this[int x, int y]
    {
        get
        {
            return this[new Vector2Int(x, y)];
        }
        set
        {
            this[new Vector2Int(x, y)] = value;
        }
    }

    public T this[Vector2Int pos]
    {
        get
        {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set
        {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }

    public string GetGridAsString()
    {
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                // Agrega el valor de la celda a la cadena, seguido de un tabulador
                sb.Append(this[x, y]);
                sb.Append("\t");
            }
            // Agrega un salto de línea después de cada fila
            sb.AppendLine();
        }

        // Devuelve la cadena completa que representa el grid
        return sb.ToString();
    }

    public void Print()
    {
        string gridString = GetGridAsString();
        Debug.Log(gridString);
    }


}
