using System;

public interface IMissingTextureManager
{
    event Action OnMissingTextureAllClear;
    void SubtractMissingTextureCount(int count = 1);
    void BindView(MissingTextureManagerView view);
    void ResetGame();
}