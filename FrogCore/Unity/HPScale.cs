using System;

namespace FrogCore.Unity;

[Serializable]
public struct HPScale
{
    public int originalHP;
    public int level1;
    public int level2;
    public int level3;

    public HPScale(int originalHP, int level1 = 0, int level2 = 0, int level3 = 0)
    {
        this.originalHP = originalHP;
        this.level1 = level1;
        this.level2 = level2;
        this.level3 = level3;
    }

    public int GetScaledHP()
    {
        if (BossSceneController.IsBossScene)
        //if (true)
        {
            switch (BossSceneController.Instance.BossLevel)
            //switch (-1)
            {
                case 0:
                    if (level1 <= 0)
                        return originalHP;
                    return level1;
                case 1:
                    if (level2 <= 0)
                        return originalHP;
                    return level2;
                case 2:
                    if (level3 <= 0)
                        return originalHP;
                    return level3;
            }
        }
        return originalHP;
    }
}