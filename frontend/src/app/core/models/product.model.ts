export interface Product {
  id: number;
  name: string;
  code: string | null;
  unit: string;
  sellingPrice: number;
  currentStock: number;
}

export interface CreateProductRequest {
  name: string;
  code: string | null;
  unit: string;
  sellingPrice: number;
  openingStock: number;
}

export interface UpdateProductRequest {
  name: string;
  code: string | null;
  unit: string;
  sellingPrice: number;
}

export interface AddStockRequest {
  quantity: number;
}

export type StockMovementReasonLabel = 'Opening Stock' | 'Sold' | 'Stock Received';

export interface StockMovement {
  id: number;
  reason: StockMovementReasonLabel;
  quantityDelta: number;
  newStock: number;
  billId: number | null;
  createdAt: string;
}
