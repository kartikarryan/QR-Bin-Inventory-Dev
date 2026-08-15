export interface LowStockPart {
  id: number;
  partName: string;
  currentStock: number;
  minimumStock: number;
  binCode: string | null;
}

export interface DashboardSummary {
  totalParts: number;
  lowStockCount: number;
  outOfStockCount: number;
  lowStockParts: LowStockPart[];
}
