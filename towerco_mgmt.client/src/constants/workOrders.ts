export const WORK_ORDER_STATUSES = ['Open', 'Assigned', 'InProgress', 'OnHold', 'Completed', 'Cancelled'] as const;
export const WORK_ORDER_PRIORITIES = ['Low', 'Medium', 'High', 'Critical'] as const;
export const WORK_ORDER_CATEGORIES = ['Preventive', 'Corrective', 'Installation', 'Inspection'] as const;

type Status = (typeof WORK_ORDER_STATUSES)[number];

// Single source of truth for the status state machine.
export const WORK_ORDER_TRANSITIONS: Record<Status, readonly Status[]> = {
    Open: ['Assigned', 'Cancelled'],
    Assigned: ['InProgress', 'OnHold', 'Cancelled'],
    InProgress: ['OnHold', 'Completed', 'Cancelled'],
    OnHold: ['InProgress', 'Cancelled'],
    Completed: [],
    Cancelled: [],
};

export const CLOSED_WORK_ORDER_STATUSES: readonly Status[] = ['Completed', 'Cancelled'];
export const ASSIGNEE_REQUIRED_STATUS: Status = 'Assigned';

export const WORK_ORDER_STATUS_LABELS: Record<Status, string> = {
    Open: 'Open', Assigned: 'Assigned', InProgress: 'In progress', OnHold: 'On hold', Completed: 'Completed', Cancelled: 'Cancelled',
};

export const WORK_ORDER_CODE_PREFIX = 'WO-';
export const WORK_ORDER_CODE_DIGITS = 4;
export const DUE_SOON_DAYS = 7;
export const DEFAULT_DUE_IN_DAYS = 14;
export const MIN_TITLE_LENGTH = 5;

export const FIELD_ENGINEERS = ['Arjun Mehta', 'Thandi Nkosi', 'Wanjiru Kamau', 'Chidi Okafor', 'Priya Nair'] as const;
