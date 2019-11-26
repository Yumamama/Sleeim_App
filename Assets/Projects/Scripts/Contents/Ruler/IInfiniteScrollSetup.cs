/**
 * @file
 * @brief IInfiniteScrollSetup
 */
using UnityEngine;

/// <summary>
/// IInfiniteScrollSetup
/// </summary>
public interface IInfiniteScrollSetup
{
	void OnPostSetupItems();
	void OnUpdateItem(int itemCount, GameObject obj);
}
