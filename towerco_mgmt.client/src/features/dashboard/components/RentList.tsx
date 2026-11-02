import { Fragment } from 'react';
import { DISPLAY_LOCALE } from '@/shared/constants/domain';
import type { CurrencyAmount } from '@/features/dashboard/types';

const format = (r: CurrencyAmount) =>
    r.amount.toLocaleString(DISPLAY_LOCALE, { style: 'currency', currency: r.currency, maximumFractionDigits: 0 });

function RentList({ rows }: { rows: CurrencyAmount[] }) {
    return (
        <dl className="details">
            {rows.map(r => <Fragment key={r.currency}><dt>{r.currency}</dt><dd>{format(r)}</dd></Fragment>)}
        </dl>
    );
}

export default RentList;
