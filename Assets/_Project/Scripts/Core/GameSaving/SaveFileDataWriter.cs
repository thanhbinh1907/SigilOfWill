using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq.Expressions;

namespace SG
{
	public class SaveFileDataWriter
	{
		public string saveDataDirectoryPath = "";
		public string saveFileName = "Character";

		// BEFORE WE CREATE A NEW FILE, WE MUST CHECK TO SEE IF A FILE WITH THE SAME NAME ALREADY EXISTS
		public bool CheckToSeeIfFileExists()
		{
			return File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName));
		}

		// USED TO DELETE CHARATER SAVE FILES
		public void DeleteSaveFile()
		{
			File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
		}

		// USED TO CREATE A NEW SAVE FILE FOR THE CHARACTER, AND WRITE THE CHARACTER SAVE DATA TO IT ASYNCHRONOUSLY
		public void CreateNewCharacterSaveFile(CharacterSaveData characterSaveData)
		{
			// MAKE A PATH TO THE SAVE FILE
			string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(savePath));

				// SERIALIZE THE CHARACTER SAVE DATA ON THE MAIN THREAD (Fast & Safe)
				string dataToStore = JsonUtility.ToJson(characterSaveData, true);

				// WRITE THE JSON STRING TO THE SAVE FILE ON A BACKGROUND THREAD (Avoids Lag Spikes)
				System.Threading.Tasks.Task.Run(() =>
				{
					try
					{
						using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
						{
							using (StreamWriter fileWriter = new StreamWriter(fileStream))
							{
								fileWriter.Write(dataToStore);
							}
						}
					}
					catch (Exception ex)
					{
						System.Console.WriteLine("An error occurred while writing the save file asynchronously: " + ex.Message);
					}
				});
			}
			catch (Exception ex)
			{
				Debug.LogError("An error occurred while preparing the save file: " + ex.Message);
			}
		}

		// USED TO LOAD A SAVE FILE UPON LOADING A PREVIOUS GAME
		public CharacterSaveData LoadSaveFile()
		{
			CharacterSaveData characterSaveData = null;

			// MAKE A PATH TO LOAD THE SAVE FILE
			string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

			if (File.Exists(savePath))
			{
				try
				{
					string dataToLoad = "";

					using (FileStream stream = new FileStream(savePath, FileMode.Open, FileAccess.Read))
					{
						using (StreamReader reader = new StreamReader(stream))
						{
							dataToLoad = reader.ReadToEnd();
						}
					}
					characterSaveData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
				}
				catch (Exception ex)
				{
					Debug.LogError("An error occurred while loading the save file: " + ex.Message);
				}
			}
			return characterSaveData;
		}


	}
}