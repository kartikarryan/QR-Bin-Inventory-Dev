export interface ApiResponse<T> {
  statusCode: number;
  message: string;
  data: T | null;
  errors: string[] | null;
}
