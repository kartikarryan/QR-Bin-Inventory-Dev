export interface BillItem {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Bill {
  id: number;
  customerName: string | null;
  customerPhone: string | null;
  total: number;
  createdAt: string;
  items: BillItem[];
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
  items: CreateBillItemRequest[];
}
