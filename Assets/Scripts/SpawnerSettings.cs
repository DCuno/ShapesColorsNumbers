using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner Settings", menuName = "SpawnerSettings")]
public class SpawnerSettings : ScriptableObject
{
    public List<Spawner.Shape> Shapes;
    public List<Spawner.Colors> Colors;
    [Range(1,10)]
    public int Size;
    [Range(1,100)]
    public int Amount;
    public bool Edges;
    public bool Tilt;
    public Spawner.Topics Topic;
    public bool Voice;
    public bool Text;
}
