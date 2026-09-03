using System.Collections;
using UnityEngine;
using MisprintDepartment.Core;

namespace MisprintDepartment.Game
{
    /// <summary>
    /// Visual representation of the proof board: a grid of cell tiles, start and
    /// destination markers, and the moving carriage. The grid parent and marker
    /// prefabs are authored in the scene; tiles are spawned per level from a
    /// single tile prefab (dynamic level content, allowed by the UI rule).
    /// </summary>
    public sealed class RouteView : MonoBehaviour
    {
        [SerializeField] Transform gridRoot;
        [SerializeField] GameObject cellTilePrefab;
        [SerializeField] GameObject blockedTilePrefab;
        [SerializeField] Transform startMarker;
        [SerializeField] Transform destinationMarker;
        [SerializeField] Transform carriage;
        [SerializeField] float cellSize = 1f;
        [SerializeField] float tileY = 0f;
        [SerializeField] float carriageY = 0.15f;

        BoardConfig _board;

        public void Build(BoardConfig board)
        {
            _board = board;
            if (gridRoot != null)
            {
                for (int i = gridRoot.childCount - 1; i >= 0; i--)
                    Destroy(gridRoot.GetChild(i).gameObject);

                for (int x = 0; x < board.width; x++)
                for (int y = 0; y < board.height; y++)
                {
                    var cell = new Cell(x, y);
                    var prefab = board.IsBlocked(cell) ? (blockedTilePrefab ?? cellTilePrefab) : cellTilePrefab;
                    if (prefab == null) continue;
                    var tile = Instantiate(prefab, gridRoot);
                    tile.transform.localPosition = LocalPos(cell, tileY);
                    tile.name = $"Cell_{x}_{y}";
                }
            }

            Center(board);
            if (startMarker != null) startMarker.localPosition = LocalPos(board.start, tileY + 0.02f);
            if (destinationMarker != null) destinationMarker.localPosition = LocalPos(board.destination, tileY + 0.02f);
            ResetCarriage(board);
        }

        public void ResetCarriage(BoardConfig board)
        {
            if (carriage == null) return;
            carriage.localPosition = LocalPos(board.start, carriageY);
            carriage.localRotation = FacingRotation(board.startFacing);
        }

        /// <summary>Animate the carriage for one executed step.</summary>
        public IEnumerator PlayStep(StepTrace step, float seconds)
        {
            if (carriage == null) { yield return new WaitForSeconds(seconds); yield break; }

            var targetPos = LocalPos(step.cell, carriageY);
            var targetRot = FacingRotation(step.facing);
            var fromPos = carriage.localPosition;
            var fromRot = carriage.localRotation;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(0.05f, seconds);
                float e = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
                carriage.localPosition = Vector3.Lerp(fromPos, targetPos, e);
                carriage.localRotation = Quaternion.Slerp(fromRot, targetRot, e);
                yield return null;
            }
            carriage.localPosition = targetPos;
            carriage.localRotation = targetRot;
        }

        Vector3 LocalPos(Cell c, float y)
        {
            // grid is centered; y-up in board space maps to +Z in world.
            float ox = -(_board.width - 1) * 0.5f * cellSize;
            float oz = -(_board.height - 1) * 0.5f * cellSize;
            return new Vector3(ox + c.x * cellSize, y, oz + c.y * cellSize);
        }

        void Center(BoardConfig board)
        {
            // no-op hook: LocalPos already centers. Kept for clarity/extension.
        }

        static Quaternion FacingRotation(Direction d)
        {
            switch (d)
            {
                case Direction.North: return Quaternion.Euler(0, 0, 0);
                case Direction.East: return Quaternion.Euler(0, 90, 0);
                case Direction.South: return Quaternion.Euler(0, 180, 0);
                default: return Quaternion.Euler(0, 270, 0);
            }
        }
    }
}
