using System.Collections;
using UnityEngine;
using System.Diagnostics;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {

        Process processoPython;

        void StartPython(string idioma)
        {
            if (processoPython != null && !processoPython.HasExited)
                processoPython.Kill();

            processoPython = new Process();
            processoPython.StartInfo.FileName = "python";
            processoPython.StartInfo.Arguments = $"main.py {idioma}";
            processoPython.StartInfo.WorkingDirectory = "CAMINHO/DO/TEU/PROJETO";
            processoPython.StartInfo.CreateNoWindow = true;
            processoPython.StartInfo.UseShellExecute = false;
            processoPython.Start();
        }
    }
}