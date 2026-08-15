export type InventoryStatus = 'In Stock' | 'Low Stock' | 'Out of Stock';

export interface InventoryPart {
  id: number;
  partName: string;
  partNumber: string | null;
  binCode: string | null;
  qrToken: string | null;
  currentStock: number;
  minimumStock: number;
  status: InventoryStatus;
}

export interface CreatePartRequest {
  partName: string;
  partNumber: string | null;
  binCode: string;
  currentStock: number;
  minimumStock: number;
}

export interface UpdatePartRequest {
  partName: string;
  partNumber: string | null;
  binCode: string;
  currentStock: number;
  minimumStock: number;
}

export interface StockMovementRequest {
  quantityDelta: number;
}
