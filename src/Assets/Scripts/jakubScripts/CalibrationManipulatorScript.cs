/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// ovl�d� manipul�tor pro rotaci, tak aby z�stal v�dy na sv�m m�st� p�i nastavov�n� rotace
/// </summary>

using UnityEngine;

public class CalibrationManipulatorScript : MonoBehaviour
{

    bool rotationMan = false;
    Vector3 lastKnownPos = Vector3.zero;
    void Update()
    {
        if (rotationMan)
        {
            this.transform.position = lastKnownPos;

        }
        else
        {
            lastKnownPos = this.transform.position;
        }

    }

    public void onRotationManStart()
    {
        rotationMan = true;
    }

    public void onRotationManStop()
    {
        rotationMan = false;
    }
}
