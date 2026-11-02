import { DISPLAY_LOCALE } from '@/shared/constants/domain';

export function formatMoney(amount: number, currency: string): string {
    return new Intl.NumberFormat(DISPLAY_LOCALE, { style: 'currency', currency }).format(amount);
}

export function countLabel(count: number | undefined, singular: string, plural: string): string | undefined {
    if (count === undefined) return undefined;
    return `${count} ${count === 1 ? singular : plural}`;
}
