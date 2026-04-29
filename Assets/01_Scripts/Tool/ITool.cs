using System;

public interface ITool : IEquipable
{
    void Init(ToolData data);
    void Use();
}