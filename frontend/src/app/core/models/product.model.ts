export const PRODUCT_UNITS = ['pcs', 'box', 'kg', 'litre', 'metre', 'set', 'other'] as const;
export type ProductUnit = (typeof PRODUCT_UNITS)[number];

export const GST_RATES = [0, 5, 12, 18, 28] as const;

export interface Product {
  id: number;
  name: string;
  code: string | null;
  hsnCode: string | null;
  unit: string;
  sellingPrice: number;
  gstRate: number;
  currentStock: number;
  isActive: boolean;
}

export interface CreateProductRequest {
  name: string;
  code: string | null;
  hsnCode: string | null;
  unit: string;
  sellingPrice: number;
  gstRate: number;
  openingStock: number;
}

export interface UpdateProductRequest {
  name: string;
  code: string | null;
  hsnCode: string | null;
  unit: string;
  sellingPrice: number;
  gstRate: number;
}

export interface SetProductActiveRequest {
  isActive: boolean;
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

/** Cross-product movement log row — unlike StockMovement, the product isn't already
 *  implied by the caller, so this carries the product's identity too. */
export interface StockMovementLogEntry {
  id: number;
  productId: number;
  productName: string;
  reason: StockMovementReasonLabel;
  quantityDelta: number;
  newStock: number;
  billId: number | null;
  createdAt: string;
}
