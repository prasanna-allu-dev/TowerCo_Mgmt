import { operatorsApi } from '@/features/operators/api';
import { Card, Pagination, StateMessage } from '@/shared/components';
import { useApi } from '@/shared/hooks/useApi';
import { usePagination } from '@/shared/hooks/usePagination';
import PageHeader from '@/shared/components/PageHeader';
import { countLabel } from '@/shared/utils/format';
import type { Operator } from '@/features/operators/types';

function OperatorsTable({ operators }: { operators: Operator[] }) {
    const paging = usePagination(operators);
    return (
        <>
            <table className="table">
                <thead><tr><th>Name</th><th>Country</th></tr></thead>
                <tbody>{paging.pageRows.map(o => <tr key={o.id}><td>{o.name}</td><td>{o.countryCode}</td></tr>)}</tbody>
            </table>
            <Pagination {...paging} />
        </>
    );
}

function OperatorsPage() {
    const { data, error, loading } = useApi(() => operatorsApi.list());

    return (
        <>
            <PageHeader title="Operators" subtitle="Mobile network operators that can lease tower slots" />
            <Card title="All operators" subtitle={countLabel(data?.length, 'operator', 'operators')}>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No operators found." />
                {!!data?.length && <OperatorsTable operators={data} />}
            </Card>
        </>
    );
}

export default OperatorsPage;
