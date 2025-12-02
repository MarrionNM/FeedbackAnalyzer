export interface PagedResult<T> {
  data: T[];
  currentPage?: number;
  pageSize?: number;
  totalRecords?: number;
  totalPages?: number;
  RecordsOnPage?: number;
}
