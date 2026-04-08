using UnityEngine;

public class GLTesting : MonoBehaviour
{
	public OnSaleDialog m_saleDialog;

	private void Start()
	{
		Invoke("ShowDialog", 3f);
	}

	private void ShowDialog()
	{
		StoreProductManager.Instance.UpdateProductsFromBedrock();
		m_saleDialog.ShowItemsOnSale();
	}

	private void Update()
	{
	}
}
