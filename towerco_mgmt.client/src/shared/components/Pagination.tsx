import type { PaginationState } from '@/shared/hooks/usePagination';
import { PAGE_SIZE_OPTIONS } from '@/shared/constants/pagination';
import SelectField from './SelectField';

type PaginationProps = Omit<PaginationState<unknown>, 'pageRows'>;

const SIZE_OPTIONS = PAGE_SIZE_OPTIONS.map(String);

const rangeLabel = ({ page, pageSize, total }: PaginationProps) =>
    `Showing ${(page - 1) * pageSize + 1} to ${Math.min(page * pageSize, total)} of ${total}`;

function Pagination(props: PaginationProps) {
    const { page, pageCount, pageSize, total, setPage, setPageSize } = props;
    if (total <= PAGE_SIZE_OPTIONS[0]) return null;
    return (
        <nav className="pagination" aria-label="Pagination">
            <span className="muted">{rangeLabel(props)}</span>
            <SelectField className="pagination-size" label="Rows" value={String(pageSize)} options={SIZE_OPTIONS} onChange={v => setPageSize(Number(v))} />
            <button type="button" className="btn btn-secondary" disabled={page <= 1} onClick={() => setPage(page - 1)}>Previous</button>
            <span>Page {page} of {pageCount}</span>
            <button type="button" className="btn btn-secondary" disabled={page >= pageCount} onClick={() => setPage(page + 1)}>Next</button>
        </nav>
    );
}

export default Pagination;
