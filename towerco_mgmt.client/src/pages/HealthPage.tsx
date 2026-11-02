import { useState } from 'react';
import { healthApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { HealthFilters, HealthKpis, HealthTable } from '@/components/health';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import type { HealthFilter } from '@/types';

function HealthPage() {
    const [filter, setFilter] = useState<HealthFilter>({});
    const { data, error, loading } = useApi(() => healthApi.portfolio(filter), [filter.country, filter.status]);

    return (
        <>
            <PageHeader title="Tower health" subtitle="Telemetry, inspections and open work orders per site. Worst signal sets the health." />
            {data && <HealthKpis health={data} />}
            <Card>
                <HealthFilters value={filter} onChange={setFilter} />
                <StateMessage loading={loading} error={error} empty={data?.sites.length === 0} emptyText="No sites match these filters." />
                {!!data?.sites.length && <HealthTable sites={data.sites} />}
            </Card>
        </>
    );
}

export default HealthPage;
