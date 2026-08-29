export interface BillItem {
  id: number;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
  /** How many of `quantity` have already come back via a return/exchange. */
  returnedQuantity: number;
}

export interface BillReturn {
  id: number;
  billItemId: number;
  productId: number;
  productName: string;
  returnedQuantity: number;
  replacementProductId: number | null;
  replacementProductName: string | null;
  replacementQuantity: number | null;
  notes: string | null;
  createdAt: string;
}

export interface Bill {
  id: number;
  customerName: string | null;
  customerPhone: string | null;
  subtotal: number;
  discountAmount: number;
  total: number;
  createdAt: string;
  items: BillItem[];
  returns: BillReturn[];
}

export interface CreateBillReturnRequest {
  billItemId: number;
  returnedQuantity: number;
  replacementProductId: number | null;
  replacementQuantity: number | null;
  notes: string | null;
}

export interface BillSummary {
  id: number;
  customerName: string | null;
  itemCount: number;
  total: number;
  createdAt: string;
}

export interface CreateBillItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateBillRequest {
  customerName: string | null;
  customerPhone: string | null;
  discountAmount: number;
  items: CreateBillItemRequest[];
}
