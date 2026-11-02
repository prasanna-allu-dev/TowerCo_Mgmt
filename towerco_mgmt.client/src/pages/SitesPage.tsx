import { useState } from 'react';
import { sitesApi } from '@/api';
import { Card, StateMessage } from '@/components/common';
import { SiteFilters, SitesTable } from '@/components/sites';
import { useApi } from '@/hooks/useApi';
import PageHeader from '@/layout/PageHeader';
import { countLabel } from '@/utils/format';
import type { SiteFilter } from '@/types';

function SitesPage() {
    const [filter, setFilter] = useState<SiteFilter>({});
    const { data, error, loading } = useApi(() => sitesApi.list(filter), [filter.country, filter.status, filter.search]);
    const hasRows = !loading && !error && !!data?.length;

    return (
        <>
            <PageHeader title="Sites" subtitle="Search and filter tower sites across the portfolio" />
            <Card title="Filters"><SiteFilters value={filter} onChange={setFilter} /></Card>
            <Card title="Results" subtitle={countLabel(data?.length, 'site', 'sites')}>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No sites match the filters." />
                {hasRows && <SitesTable sites={data} />}
            </Card>
        </>
    );
}

export default SitesPage;
