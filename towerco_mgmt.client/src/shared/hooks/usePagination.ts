import { useState } from 'react';
import { DEFAULT_PAGE_SIZE, FIRST_PAGE } from '@/shared/constants/pagination';

export interface PaginationState<T> {
    pageRows: T[];
    page: number;
    pageCount: number;
    pageSize: number;
    total: number;
    setPage: (page: number) => void;
    setPageSize: (size: number) => void;
}

const countPages = (total: number, size: number) => Math.max(FIRST_PAGE, Math.ceil(total / size));
const clampPage = (page: number, pageCount: number) => Math.min(Math.max(page, FIRST_PAGE), pageCount);
const sliceRows = <T,>(rows: readonly T[], page: number, size: number) => rows.slice((page - 1) * size, page * size);

export function usePagination<T>(rows: readonly T[], initialSize = DEFAULT_PAGE_SIZE): PaginationState<T> {
    const [pageSize, setSize] = useState(initialSize);
    const [page, setPage] = useState(FIRST_PAGE);
    const [prevRows, setPrevRows] = useState(rows);
    if (prevRows !== rows) {
        setPrevRows(rows);
        setPage(FIRST_PAGE);
    }
    const pageCount = countPages(rows.length, pageSize);
    const current = clampPage(page, pageCount);
    const setPageSize = (size: number) => { setSize(size); setPage(FIRST_PAGE); };
    return { pageRows: sliceRows(rows, current, pageSize), page: current, pageCount, pageSize, total: rows.length, setPage, setPageSize };
}
