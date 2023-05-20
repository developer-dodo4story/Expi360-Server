using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameRoomServer
{
    public class SerializeHelper
    {
        private static readonly string fileExtension = ".txt";

        /// <summary>
        /// Scieżka prowadzi do projektu. Jeden poziom wyżej od Assets. Uwaga! W buildzie prowadzi do folderu data.
        /// </summary>
#if UNITY_EDITOR
        public static readonly string pathToProject = Application.dataPath.Remove(Application.dataPath.Length - 7, 7);
#else
        public static readonly string pathToProject = Application.dataPath;
#endif


        /// <summary>
        /// Zapisuje obiekt na dysku do JSON. 
        /// Jeżeli ścieżka nie istnieje zostanie stworzona. 
        /// Jeżeli plik już istnieje zostanie nadpisany.
        /// </summary>
        /// <param name="fileName">Nazwa pliku bez rozszerzenia</param>
        /// <param name="path">Ścieżka do miejsca zapisu</param>
        /// <param name="objectToSave">Obiekt do serializacji</param>
        public static bool SaveToJSON<T>(string fileName, string path, T objectToSave)
        {
            string fullPath = Path.Combine(path, fileName + fileExtension);
            //Tworzy wszystkie foldery i podfoldery w określonej ścieżce, chyba że już istnieją.
            // Uwaga, odcina nazwe pliki od stringa. Jesli jej nie ma to odetnię ostatni folder.
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using (StreamWriter sw = new StreamWriter(fullPath, false, System.Text.Encoding.UTF8))
            {
                string json = JsonUtility.ToJson(objectToSave);
                try
                {
                    sw.Write(json);
                    return true;
                }
                catch (ObjectDisposedException e)
                {
                    Debug.LogError("Info: The exception that is thrown when an operation is performed on a disposed object. Caught: " + e.Message);
                }
                catch (NotSupportedException e)
                {
                    Debug.LogError("Info: The exception that is thrown when an invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality. Caught: " + e.Message);
                }
                catch (IOException e)
                {
                    Debug.LogError("Info: The exception that is thrown when an I/O error occurs. Caught: " + e.Message);
                }
            }
            return false;
        }

        /// <summary>
        /// Wczytuje plik z dysku do odpowiedniego obiektu. Jeśli udało wczytać się plik zwraca true.
        /// </summary>
        /// <param name="fileName">Nazwa pliku bez rozszerzenia</param>
        /// <param name="path">Ściezka do pliku</param>
        /// <param name="objectToLoad">Referencja do obiektu, który zostanie podmieniony na wczytany</param>
        public static bool LoadFromJSON<T>(string fileName, string path, ref T objectToLoad)
        {
            string fullPath = Path.Combine(path, fileName + fileExtension);
            if (File.Exists(fullPath))
            {
                using (StreamReader sr = new StreamReader(fullPath, System.Text.Encoding.UTF8))
                {
                    string json = "";
                    try
                    {
                        json = sr.ReadToEnd();
                    }
                    catch (OutOfMemoryException e)
                    {
                        Debug.LogError("Info: The exception that is thrown when there is not enough memory to continue the execution of a program. Caught: " + e.Message);
                    }
                    catch (IOException e)
                    {
                        Debug.LogError("Info: The exception that is thrown when an I/O error occurs. Caught: " + e.Message);
                    }
                    objectToLoad = JsonUtility.FromJson<T>(json);
                    return true;
                }
            }
            else
            {
                Debug.LogError("Podany plik nie istnieje: " + fullPath);
                return false;
            }
        }
    }
}
