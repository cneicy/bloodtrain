using UnityEngine;

namespace Build
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField] private GameObject cellIndicator;
        [SerializeField] private BuildInput inputManager;
        [SerializeField] private Grid grid;

        private void Update()
        {
            Vector3 mousePosition = inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        }
    }
}
