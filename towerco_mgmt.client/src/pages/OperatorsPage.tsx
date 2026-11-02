import { operatorsApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import { countLabel } from '@/utils/format';

function OperatorsPage() {
    const { data, error, loading } = useApi(() => operatorsApi.list());

    return (
        <>
            <PageHeader title="Operators" subtitle="Mobile network operators that can lease tower slots" />
            <Card title="All operators" subtitle={countLabel(data?.length, 'operator', 'operators')}>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No operators found." />
                {!!data?.length && (
                    <table className="table">
                        <thead><tr><th>Name</th><th>Country</th></tr></thead>
                        <tbody>{data.map(o => <tr key={o.id}><td>{o.name}</td><td>{o.countryCode}</td></tr>)}</tbody>
                    </table>
                )}
            </Card>
        </>
    );
}

export default OperatorsPage;
