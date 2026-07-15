export type LoginResponse = {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  userId: string;
  email: string;
  fullName: string;
  roles: string[];
};

export type WorkOrder = {
  id: string;
  assetId: string;
  description: string;
  status: string;
};

export type Asset = {
  id: string;
  name: string;
  serialNumber?: string | null;
  locationCode?: string | null;
  criticality: string;
};

export type Facility = {
  id: string;
  name: string;
  code?: string | null;
  address?: string | null;
  timeZone?: string | null;
  parentFacilityId?: string | null;
};

export type Part = {
  id: string;
  partNumber: string;
  name: string;
  description?: string | null;
  unitOfMeasure: string;
  isActive: boolean;
};

export type Worker = {
  id: string;
  firstName: string;
  lastName: string;
  employeeNumber?: string | null;
  trade?: string | null;
  department?: string | null;
  phone?: string | null;
  isActive: boolean;
};

export type Vendor = {
  id: string;
  name: string;
  taxNumber?: string | null;
  contactEmail?: string | null;
  phone?: string | null;
  isActive: boolean;
};

export type InventoryTransaction = {
  id: string;
  partId: string;
  partNumber: string;
  partName: string;
  type: string;
  quantity: number;
  unitOfMeasure: string;
  binId?: string | null;
  relatedWorkOrderId?: string | null;
  notes?: string | null;
  transactionDate: string;
  currentStock?: number | null;
};

export type User = {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  roles: string[];
};

export type Role = {
  id: string;
  name: string;
  description?: string | null;
  isSystem: boolean;
  permissions: string[];
};

export type Permission = {
  id: string;
  name: string;
  description?: string | null;
};

export type AutomationRule = {
  id: string;
  name: string;
  description?: string | null;
  isEnabled: boolean;
  triggerType: string;
  conditionJson: string;
  actionType: string;
  actionParametersJson: string;
};

export type AutomationRuleExecution = {
  id: string;
  ruleId: string;
  executedAt: string;
  success: boolean;
  detail: string;
};

export type PluginRegistration = {
  id: string;
  name: string;
  version: string;
  description?: string | null;
  author?: string | null;
  status: "Available" | "Enabled" | "Disabled" | string;
  sourceUrl?: string | null;
  storagePath?: string | null;
  installedAt?: string | null;
};

export type AssetMaintenanceInsight = {
  assetId: string;
  assetName: string;
  failureCount: number;
  failureFrequencyPerMonth: number;
  meanTimeBetweenFailuresDays?: number | null;
  trend: string;
  elevatedRecentFailureRate: boolean;
  risk: string;
  rationale: string;
};

export type PartReorderInsight = {
  partId: string;
  partNumber: string;
  partName: string;
  currentStock: number;
  suggestedReorderThreshold: number;
  averageMonthlyIssueQuantity: number;
  rationale: string;
};
