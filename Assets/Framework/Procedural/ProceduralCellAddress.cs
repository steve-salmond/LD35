using UnityEngine;
using System.Collections.Generic;

public struct ProceduralCellAddress
{
    public int x;
    public int y;

    public ProceduralCellAddress(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
        { return x + "," + y; }

    public override bool Equals(System.Object o)
        { return o is ProceduralCellAddress && this == (ProceduralCellAddress) o; }

    public override int GetHashCode()
        { return x ^ y; }

    public static bool operator ==(ProceduralCellAddress a, ProceduralCellAddress b)
        { return a.x == b.x && a.y == b.y; }

    public static bool operator !=(ProceduralCellAddress a, ProceduralCellAddress b)
        { return !(a == b); }
}
