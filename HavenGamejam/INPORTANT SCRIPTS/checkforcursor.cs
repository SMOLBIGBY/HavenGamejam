Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

// 2. Check if the mouse position overlaps the item's 2D collider
bool isHovered = itemCollider.OverlapPoint(mouseWorldPos);
