# BotSharp SaaS 订阅计划用户体验与前端实现指南

## 概述

本文档详细描述了 BotSharp SaaS 平台的订阅计划系统在用户体验层面的设计和前端实现方案，包括计费页面、用量仪表板、限制提示、升级流程等关键界面组件。

## 用户体验设计原则

### 1. 透明度优先
- 清晰展示当前使用情况和限制
- 提供详细的用量历史和趋势分析
- 明确说明每个限制的具体含义

### 2. 主动引导
- 在接近限制时主动提醒用户
- 提供个性化的升级建议
- 智能推荐最适合的订阅计划

### 3. 无缝体验
- 限制达到时提供优雅降级
- 升级流程简化，减少摩擦
- 实时更新用量信息

## 核心界面组件

### 1. 订阅计划选择页面

```tsx
import React, { useState, useEffect } from 'react';
import { Card, Button, Badge, Tooltip, Progress } from 'antd';
import { CheckOutlined, CrownOutlined, RocketOutlined } from '@ant-design/icons';

interface PlanFeature {
  name: string;
  value: string | number;
  tooltip?: string;
  highlight?: boolean;
}

interface SubscriptionPlan {
  id: string;
  name: string;
  price: number;
  billingCycle: 'monthly' | 'yearly';
  features: PlanFeature[];
  recommended?: boolean;
  popular?: boolean;
  currentPlan?: boolean;
}

const SubscriptionPlansPage: React.FC = () => {
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [currentUsage, setCurrentUsage] = useState<any>({});
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    loadPlansAndUsage();
  }, []);

  const loadPlansAndUsage = async () => {
    setIsLoading(true);
    try {
      const [plansResponse, usageResponse] = await Promise.all([
        fetch('/api/subscription/plans'),
        fetch('/api/usage/current')
      ]);
      
      setPlans(await plansResponse.json());
      setCurrentUsage(await usageResponse.json());
    } catch (error) {
      console.error('Failed to load plans and usage:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handlePlanSelect = async (planId: string) => {
    setIsLoading(true);
    try {
      const response = await fetch('/api/subscription/upgrade', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ planId })
      });

      if (response.ok) {
        // 跳转到支付页面或显示成功消息
        window.location.href = '/billing/upgrade-success';
      } else {
        // 处理错误
        const error = await response.json();
        console.error('Upgrade failed:', error);
      }
    } catch (error) {
      console.error('Upgrade request failed:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const renderPlanCard = (plan: SubscriptionPlan) => (
    <Card
      key={plan.id}
      className={`plan-card ${plan.recommended ? 'recommended' : ''} ${plan.currentPlan ? 'current' : ''}`}
      title={
        <div className="plan-header">
          <div className="plan-name">
            {plan.name}
            {plan.recommended && <Badge color="gold" text="推荐" />}
            {plan.popular && <Badge color="red" text="最受欢迎" />}
            {plan.currentPlan && <Badge color="green" text="当前计划" />}
          </div>
          <div className="plan-price">
            <span className="price">${plan.price}</span>
            <span className="cycle">/{plan.billingCycle === 'monthly' ? '月' : '年'}</span>
          </div>
        </div>
      }
      actions={[
        <Button
          type={plan.recommended ? 'primary' : 'default'}
          size="large"
          loading={isLoading}
          disabled={plan.currentPlan}
          onClick={() => handlePlanSelect(plan.id)}
          icon={plan.recommended ? <CrownOutlined /> : <RocketOutlined />}
        >
          {plan.currentPlan ? '当前计划' : plan.recommended ? '立即升级' : '选择计划'}
        </Button>
      ]}
    >
      <div className="plan-features">
        {plan.features.map((feature, index) => (
          <div key={index} className="feature-item">
            <CheckOutlined className="feature-icon" />
            <span className="feature-name">{feature.name}</span>
            <Tooltip title={feature.tooltip}>
              <span className={`feature-value ${feature.highlight ? 'highlight' : ''}`}>
                {feature.value}
              </span>
            </Tooltip>
            {currentUsage[feature.name.toLowerCase()] && (
              <div className="current-usage">
                <Progress
                  size="small"
                  percent={(currentUsage[feature.name.toLowerCase()] / Number(feature.value)) * 100}
                  status={currentUsage[feature.name.toLowerCase()] > Number(feature.value) * 0.8 ? 'exception' : 'active'}
                  showInfo={false}
                />
                <span className="usage-text">
                  当前使用: {currentUsage[feature.name.toLowerCase()]}/{feature.value}
                </span>
              </div>
            )}
          </div>
        ))}
      </div>
    </Card>
  );

  return (
    <div className="subscription-plans-page">
      <div className="page-header">
        <h1>选择订阅计划</h1>
        <p>根据您的需求选择最适合的计划，随时可以升级或降级</p>
      </div>

      <div className="plans-comparison">
        <div className="plans-grid">
          {plans.map(renderPlanCard)}
        </div>
      </div>

      <div className="plans-faq">
        <h3>常见问题</h3>
        <div className="faq-items">
          <div className="faq-item">
            <h4>可以随时更改计划吗？</h4>
            <p>是的，您可以随时升级或降级您的订阅计划。升级立即生效，降级将在下个计费周期生效。</p>
          </div>
          <div className="faq-item">
            <h4>超出限制会怎样？</h4>
            <p>当您接近或超出某项限制时，我们会提前通知您。部分功能可能会被暂时限制，但您的数据始终安全。</p>
          </div>
          <div className="faq-item">
            <h4>支持哪些支付方式？</h4>
            <p>我们支持信用卡、PayPal、银行转账等多种支付方式。企业客户还可以申请发票付款。</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SubscriptionPlansPage;
```

### 2. 用量仪表板组件

```tsx
import React, { useState, useEffect } from 'react';
import { Card, Progress, Statistic, Alert, Button, Tooltip, Select, DatePicker } from 'antd';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip as RechartsTooltip, ResponsiveContainer } from 'recharts';
import { WarningOutlined, InfoCircleOutlined, TrendingUpOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';

interface UsageMetric {
  resource: string;
  displayName: string;
  current: number;
  limit: number;
  unit: string;
  resetTime: string;
  trend: 'up' | 'down' | 'stable';
  status: 'normal' | 'warning' | 'critical' | 'exceeded';
}

interface UsageHistoryPoint {
  timestamp: string;
  value: number;
}

const UsageDashboard: React.FC = () => {
  const [metrics, setMetrics] = useState<UsageMetric[]>([]);
  const [selectedMetric, setSelectedMetric] = useState<string>('api_calls');
  const [historyData, setHistoryData] = useState<UsageHistoryPoint[]>([]);
  const [timeRange, setTimeRange] = useState<'7d' | '30d' | '90d'>('7d');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    loadUsageMetrics();
  }, []);

  useEffect(() => {
    loadUsageHistory(selectedMetric, timeRange);
  }, [selectedMetric, timeRange]);

  const loadUsageMetrics = async () => {
    setIsLoading(true);
    try {
      const response = await fetch('/api/usage/metrics');
      const data = await response.json();
      setMetrics(data);
    } catch (error) {
      console.error('Failed to load usage metrics:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const loadUsageHistory = async (metric: string, range: string) => {
    setIsLoading(true);
    try {
      const response = await fetch(`/api/usage/history?metric=${metric}&range=${range}`);
      const data = await response.json();
      setHistoryData(data);
    } catch (error) {
      console.error('Failed to load usage history:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'normal': return '#52c41a';
      case 'warning': return '#faad14';
      case 'critical': return '#ff4d4f';
      case 'exceeded': return '#ff7875';
      default: return '#d9d9d9';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'normal': return '正常';
      case 'warning': return '接近限制';
      case 'critical': return '即将超限';
      case 'exceeded': return '已超限';
      default: return '未知';
    }
  };

  const getProgressStatus = (status: string) => {
    switch (status) {
      case 'normal': return 'success';
      case 'warning': return 'active';
      case 'critical': return 'exception';
      case 'exceeded': return 'exception';
      default: return 'normal';
    }
  };

  const renderMetricCard = (metric: UsageMetric) => {
    const usagePercentage = (metric.current / metric.limit) * 100;
    const resetTime = dayjs(metric.resetTime).format('YYYY-MM-DD HH:mm');

    return (
      <Card
        key={metric.resource}
        className={`usage-metric-card ${metric.status}`}
        title={
          <div className="metric-header">
            <span>{metric.displayName}</span>
            <Tooltip title={`重置时间: ${resetTime}`}>
              <InfoCircleOutlined className="info-icon" />
            </Tooltip>
          </div>
        }
        extra={
          <Button
            size="small"
            type="link"
            onClick={() => setSelectedMetric(metric.resource)}
          >
            查看详情
          </Button>
        }
      >
        <div className="metric-content">
          <div className="metric-stats">
            <Statistic
              title="当前使用"
              value={metric.current}
              suffix={`/ ${metric.limit} ${metric.unit}`}
              valueStyle={{ color: getStatusColor(metric.status) }}
            />
          </div>

          <div className="metric-progress">
            <Progress
              percent={Math.min(usagePercentage, 100)}
              status={getProgressStatus(metric.status)}
              strokeColor={getStatusColor(metric.status)}
              trailColor="#f5f5f5"
            />
            <div className="progress-info">
              <span className="usage-percentage">{usagePercentage.toFixed(1)}%</span>
              <span className="status-text">{getStatusText(metric.status)}</span>
            </div>
          </div>

          {metric.status !== 'normal' && (
            <Alert
              type={metric.status === 'exceeded' ? 'error' : 'warning'}
              showIcon
              message={
                metric.status === 'exceeded'
                  ? '已超出使用限制'
                  : metric.status === 'critical'
                  ? '即将达到使用限制'
                  : '接近使用限制'
              }
              description={
                metric.status === 'exceeded'
                  ? '部分功能可能受到限制，建议升级订阅计划'
                  : '建议关注使用情况或考虑升级计划'
              }
              action={
                <Button size="small" type="primary">
                  升级计划
                </Button>
              }
            />
          )}

          <div className="metric-trend">
            <TrendingUpOutlined
              className={`trend-icon ${metric.trend}`}
              style={{
                color: metric.trend === 'up' ? '#ff4d4f' :
                       metric.trend === 'down' ? '#52c41a' : '#d9d9d9'
              }}
            />
            <span className="trend-text">
              {metric.trend === 'up' ? '使用量上升' :
               metric.trend === 'down' ? '使用量下降' : '使用量稳定'}
            </span>
          </div>
        </div>
      </Card>
    );
  };

  return (
    <div className="usage-dashboard">
      <div className="dashboard-header">
        <h2>使用情况概览</h2>
        <Button type="primary" onClick={loadUsageMetrics}>
          刷新数据
        </Button>
      </div>

      {/* 使用限制警告 */}
      {metrics.some(m => m.status === 'critical' || m.status === 'exceeded') && (
        <Alert
          type="warning"
          showIcon
          icon={<WarningOutlined />}
          message="注意：部分资源使用量接近或超出限制"
          description="为确保服务正常运行，请考虑升级您的订阅计划或优化资源使用。"
          action={
            <Button type="primary" size="small">
              查看升级选项
            </Button>
          }
          style={{ marginBottom: 24 }}
        />
      )}

      {/* 指标卡片网格 */}
      <div className="metrics-grid">
        {metrics.map(renderMetricCard)}
      </div>

      {/* 使用趋势图表 */}
      <Card
        title="使用趋势分析"
        extra={
          <div className="chart-controls">
            <Select
              value={selectedMetric}
              onChange={setSelectedMetric}
              style={{ width: 120, marginRight: 16 }}
            >
              {metrics.map(metric => (
                <Select.Option key={metric.resource} value={metric.resource}>
                  {metric.displayName}
                </Select.Option>
              ))}
            </Select>
            <Select
              value={timeRange}
              onChange={setTimeRange}
              style={{ width: 100 }}
            >
              <Select.Option value="7d">7天</Select.Option>
              <Select.Option value="30d">30天</Select.Option>
              <Select.Option value="90d">90天</Select.Option>
            </Select>
          </div>
        }
      >
        <ResponsiveContainer width="100%" height={300}>
          <LineChart data={historyData}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis
              dataKey="timestamp"
              tickFormatter={tick => dayjs(tick).format('MM/DD')}
            />
            <YAxis />
            <RechartsTooltip
              labelFormatter={label => dayjs(label).format('YYYY-MM-DD HH:mm')}
              formatter={value => [value, '使用量']}
            />
            <Line
              type="monotone"
              dataKey="value"
              stroke="#1890ff"
              strokeWidth={2}
              dot={{ fill: '#1890ff', strokeWidth: 2, r: 4 }}
              activeDot={{ r: 6 }}
            />
          </LineChart>
        </ResponsiveContainer>
      </Card>

      {/* 使用建议 */}
      <Card title="优化建议" className="usage-recommendations">
        <div className="recommendations-list">
          <div className="recommendation-item">
            <h4>API 调用优化</h4>
            <p>实施缓存策略可以减少 30-50% 的 API 调用次数，建议对常用数据进行缓存。</p>
          </div>
          <div className="recommendation-item">
            <h4>Agent 管理</h4>
            <p>定期清理不活跃的 Agent 可以释放资源配额，建议每月审查 Agent 使用情况。</p>
          </div>
          <div className="recommendation-item">
            <h4>对话归档</h4>
            <p>启用自动对话归档功能，可以有效管理存储空间并提升系统性能。</p>
          </div>
        </div>
      </Card>
    </div>
  );
};

export default UsageDashboard;
```

### 3. 限制提示组件

```tsx
import React, { useState, useEffect } from 'react';
import { Modal, Alert, Button, Progress, Descriptions, Typography } from 'antd';
import { ExclamationCircleOutlined, UpgradeOutlined, SettingOutlined } from '@ant-design/icons';

const { Text, Link } = Typography;

interface LimitReachedModalProps {
  visible: boolean;
  onClose: () => void;
  limitInfo: {
    resource: string;
    displayName: string;
    current: number;
    limit: number;
    unit: string;
    resetTime: string;
    severity: 'warning' | 'error';
  };
}

const LimitReachedModal: React.FC<LimitReachedModalProps> = ({
  visible,
  onClose,
  limitInfo
}) => {
  const [upgradeOptions, setUpgradeOptions] = useState<any[]>([]);

  useEffect(() => {
    if (visible) {
      loadUpgradeOptions();
    }
  }, [visible]);

  const loadUpgradeOptions = async () => {
    try {
      const response = await fetch('/api/subscription/upgrade-options');
      const data = await response.json();
      setUpgradeOptions(data);
    } catch (error) {
      console.error('Failed to load upgrade options:', error);
    }
  };

  const handleUpgrade = (planId: string) => {
    window.location.href = `/billing/upgrade?plan=${planId}`;
  };

  const usagePercentage = (limitInfo.current / limitInfo.limit) * 100;

  return (
    <Modal
      title={
        <div style={{ display: 'flex', alignItems: 'center' }}>
          <ExclamationCircleOutlined
            style={{
              color: limitInfo.severity === 'error' ? '#ff4d4f' : '#faad14',
              marginRight: 8
            }}
          />
          {limitInfo.severity === 'error' ? '使用限制已达到' : '接近使用限制'}
        </div>
      }
      open={visible}
      onCancel={onClose}
      footer={[
        <Button key="close" onClick={onClose}>
          稍后处理
        </Button>,
        <Button
          key="optimize"
          icon={<SettingOutlined />}
          onClick={() => window.location.href = '/settings/optimization'}
        >
          优化使用
        </Button>,
        <Button
          key="upgrade"
          type="primary"
          icon={<UpgradeOutlined />}
          onClick={() => window.location.href = '/billing/plans'}
        >
          升级计划
        </Button>
      ]}
      width={600}
    >
      <div style={{ marginBottom: 16 }}>
        <Alert
          type={limitInfo.severity}
          showIcon
          message={
            limitInfo.severity === 'error'
              ? `您的 ${limitInfo.displayName} 使用量已达到当前计划限制`
              : `您的 ${limitInfo.displayName} 使用量接近当前计划限制`
          }
          description={
            limitInfo.severity === 'error'
              ? '为了确保服务正常运行，某些功能可能会被暂时限制。请考虑升级您的订阅计划。'
              : '建议您关注使用情况，避免超出限制影响服务使用。'
          }
        />
      </div>

      <Descriptions title="使用详情" bordered size="small">
        <Descriptions.Item label="资源类型">{limitInfo.displayName}</Descriptions.Item>
        <Descriptions.Item label="当前使用">
          <Text strong style={{ color: limitInfo.severity === 'error' ? '#ff4d4f' : '#faad14' }}>
            {limitInfo.current.toLocaleString()} {limitInfo.unit}
          </Text>
        </Descriptions.Item>
        <Descriptions.Item label="计划限制">
          {limitInfo.limit.toLocaleString()} {limitInfo.unit}
        </Descriptions.Item>
        <Descriptions.Item label="使用率" span={2}>
          <Progress
            percent={Math.min(usagePercentage, 100)}
            status={limitInfo.severity === 'error' ? 'exception' : 'active'}
            strokeColor={limitInfo.severity === 'error' ? '#ff4d4f' : '#faad14'}
          />
        </Descriptions.Item>
        <Descriptions.Item label="重置时间">
          {new Date(limitInfo.resetTime).toLocaleString()}
        </Descriptions.Item>
      </Descriptions>

      {upgradeOptions.length > 0 && (
        <div style={{ marginTop: 24 }}>
          <h4>推荐升级选项</h4>
          {upgradeOptions.map(option => (
            <div
              key={option.id}
              style={{
                border: '1px solid #d9d9d9',
                borderRadius: 6,
                padding: 16,
                marginBottom: 12,
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
              }}
            >
              <div>
                <Text strong>{option.name}</Text>
                <div style={{ marginTop: 4 }}>
                  <Text type="secondary">
                    {limitInfo.displayName}: {option.limits[limitInfo.resource]?.toLocaleString()} {limitInfo.unit}
                  </Text>
                </div>
                <div style={{ marginTop: 4 }}>
                  <Text style={{ fontSize: 18, fontWeight: 'bold' }}>
                    ${option.price}/{option.billingCycle === 'monthly' ? '月' : '年'}
                  </Text>
                </div>
              </div>
              <Button
                type="primary"
                onClick={() => handleUpgrade(option.id)}
              >
                立即升级
              </Button>
            </div>
          ))}
        </div>
      )}

      <div style={{ marginTop: 16, padding: 12, backgroundColor: '#f6f8fa', borderRadius: 6 }}>
        <Text type="secondary" style={{ fontSize: 12 }}>
          <strong>优化建议：</strong>
          {limitInfo.resource === 'api_calls' && '考虑实施API缓存策略，避免重复调用'}
          {limitInfo.resource === 'agents' && '清理不活跃的Agent，释放配额空间'}
          {limitInfo.resource === 'conversations' && '启用对话自动归档功能'}
          {limitInfo.resource === 'storage' && '清理历史文件和数据，释放存储空间'}
        </Text>
      </div>
    </Modal>
  );
};

export default LimitReachedModal;
```

### 4. 计费管理页面

```tsx
import React, { useState, useEffect } from 'react';
import { Card, Table, Button, Tag, Descriptions, Modal, Alert, Divider } from 'antd';
import { DownloadOutlined, EyeOutlined, CreditCardOutlined } from '@ant-design/icons';

interface Invoice {
  id: string;
  number: string;
  date: string;
  dueDate: string;
  amount: number;
  status: 'paid' | 'pending' | 'overdue';
  items: InvoiceItem[];
}

interface InvoiceItem {
  description: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

interface PaymentMethod {
  id: string;
  type: 'card' | 'paypal' | 'bank';
  last4?: string;
  brand?: string;
  email?: string;
  isDefault: boolean;
  expiryDate?: string;
}

const BillingPage: React.FC = () => {
  const [currentSubscription, setCurrentSubscription] = useState<any>(null);
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [paymentMethods, setPaymentMethods] = useState<PaymentMethod[]>([]);
  const [selectedInvoice, setSelectedInvoice] = useState<Invoice | null>(null);
  const [isInvoiceModalVisible, setIsInvoiceModalVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    loadBillingData();
  }, []);

  const loadBillingData = async () => {
    setIsLoading(true);
    try {
      const [subscriptionResponse, invoicesResponse, paymentMethodsResponse] = await Promise.all([
        fetch('/api/subscription/current'),
        fetch('/api/billing/invoices'),
        fetch('/api/billing/payment-methods')
      ]);

      setCurrentSubscription(await subscriptionResponse.json());
      setInvoices(await invoicesResponse.json());
      setPaymentMethods(await paymentMethodsResponse.json());
    } catch (error) {
      console.error('Failed to load billing data:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleViewInvoice = (invoice: Invoice) => {
    setSelectedInvoice(invoice);
    setIsInvoiceModalVisible(true);
  };

  const handleDownloadInvoice = async (invoiceId: string) => {
    try {
      const response = await fetch(`/api/billing/invoices/${invoiceId}/download`);
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `invoice-${invoiceId}.pdf`;
      a.click();
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Failed to download invoice:', error);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'paid': return 'green';
      case 'pending': return 'orange';
      case 'overdue': return 'red';
      default: return 'default';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'paid': return '已支付';
      case 'pending': return '待支付';
      case 'overdue': return '逾期';
      default: return '未知';
    }
  };

  const invoiceColumns = [
    {
      title: '发票编号',
      dataIndex: 'number',
      key: 'number',
      render: (text: string, record: Invoice) => (
        <Button type="link" onClick={() => handleViewInvoice(record)}>
          {text}
        </Button>
      )
    },
    {
      title: '开票日期',
      dataIndex: 'date',
      key: 'date',
      render: (date: string) => new Date(date).toLocaleDateString()
    },
    {
      title: '到期日期',
      dataIndex: 'dueDate',
      key: 'dueDate',
      render: (date: string) => new Date(date).toLocaleDateString()
    },
    {
      title: '金额',
      dataIndex: 'amount',
      key: 'amount',
      render: (amount: number) => `$${amount.toFixed(2)}`
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={getStatusColor(status)}>
          {getStatusText(status)}
        </Tag>
      )
    },
    {
      title: '操作',
      key: 'actions',
      render: (_, record: Invoice) => (
        <div>
          <Button
            type="link"
            icon={<EyeOutlined />}
            onClick={() => handleViewInvoice(record)}
          >
            查看
          </Button>
          <Button
            type="link"
            icon={<DownloadOutlined />}
            onClick={() => handleDownloadInvoice(record.id)}
          >
            下载
          </Button>
        </div>
      )
    }
  ];

  const renderPaymentMethod = (method: PaymentMethod) => (
    <Card key={method.id} size="small" style={{ marginBottom: 12 }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div style={{ display: 'flex', alignItems: 'center' }}>
          <CreditCardOutlined style={{ marginRight: 8, fontSize: 16 }} />
          <div>
            {method.type === 'card' && (
              <div>
                <Text strong>{method.brand?.toUpperCase()} •••• {method.last4}</Text>
                <div style={{ fontSize: 12, color: '#666' }}>
                  到期: {method.expiryDate}
                </div>
              </div>
            )}
            {method.type === 'paypal' && (
              <Text strong>PayPal - {method.email}</Text>
            )}
            {method.type === 'bank' && (
              <Text strong>银行转账</Text>
            )}
          </div>
        </div>
        <div>
          {method.isDefault && (
            <Tag color="blue" style={{ marginRight: 8 }}>默认</Tag>
          )}
          <Button size="small">编辑</Button>
        </div>
      </div>
    </Card>
  );

  return (
    <div className="billing-page">
      <div className="page-header">
        <h1>计费管理</h1>
        <Button type="primary" onClick={() => window.location.href = '/billing/plans'}>
          更改计划
        </Button>
      </div>

      {/* 当前订阅信息 */}
      {currentSubscription && (
        <Card title="当前订阅" style={{ marginBottom: 24 }}>
          <Descriptions column={2}>
            <Descriptions.Item label="计划名称">
              {currentSubscription.planName}
            </Descriptions.Item>
            <Descriptions.Item label="计费周期">
              {currentSubscription.billingCycle === 'monthly' ? '月付' : '年付'}
            </Descriptions.Item>
            <Descriptions.Item label="价格">
              ${currentSubscription.price}/{currentSubscription.billingCycle === 'monthly' ? '月' : '年'}
            </Descriptions.Item>
            <Descriptions.Item label="下次计费日期">
              {new Date(currentSubscription.nextBillingDate).toLocaleDateString()}
            </Descriptions.Item>
            <Descriptions.Item label="状态">
              <Tag color={currentSubscription.status === 'active' ? 'green' : 'red'}>
                {currentSubscription.status === 'active' ? '有效' : '无效'}
              </Tag>
            </Descriptions.Item>
          </Descriptions>
        </Card>
      )}

      {/* 待支付提醒 */}
      {invoices.some(inv => inv.status === 'pending' || inv.status === 'overdue') && (
        <Alert
          type="warning"
          showIcon
          message="您有待支付的发票"
          description="请及时处理待支付的发票，避免服务中断。"
          action={
            <Button size="small" type="primary">
              立即支付
            </Button>
          }
          style={{ marginBottom: 24 }}
        />
      )}

      <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: 24 }}>
        {/* 发票历史 */}
        <Card title="发票历史">
          <Table
            columns={invoiceColumns}
            dataSource={invoices}
            rowKey="id"
            loading={isLoading}
            pagination={{ pageSize: 10 }}
          />
        </Card>

        {/* 支付方式 */}
        <Card
          title="支付方式"
          extra={<Button type="link">添加支付方式</Button>}
        >
          {paymentMethods.map(renderPaymentMethod)}
          {paymentMethods.length === 0 && (
            <div style={{ textAlign: 'center', padding: 20, color: '#666' }}>
              暂无支付方式
              <br />
              <Button type="primary" style={{ marginTop: 12 }}>
                添加支付方式
              </Button>
            </div>
          )}
        </Card>
      </div>

      {/* 发票详情模态框 */}
      <Modal
        title={`发票详情 - ${selectedInvoice?.number}`}
        open={isInvoiceModalVisible}
        onCancel={() => setIsInvoiceModalVisible(false)}
        footer={[
          <Button key="close" onClick={() => setIsInvoiceModalVisible(false)}>
            关闭
          </Button>,
          <Button
            key="download"
            type="primary"
            icon={<DownloadOutlined />}
            onClick={() => selectedInvoice && handleDownloadInvoice(selectedInvoice.id)}
          >
            下载PDF
          </Button>
        ]}
        width={700}
      >
        {selectedInvoice && (
          <div>
            <Descriptions bordered size="small" style={{ marginBottom: 16 }}>
              <Descriptions.Item label="发票编号">{selectedInvoice.number}</Descriptions.Item>
              <Descriptions.Item label="开票日期">
                {new Date(selectedInvoice.date).toLocaleDateString()}
              </Descriptions.Item>
              <Descriptions.Item label="到期日期">
                {new Date(selectedInvoice.dueDate).toLocaleDateString()}
              </Descriptions.Item>
              <Descriptions.Item label="状态">
                <Tag color={getStatusColor(selectedInvoice.status)}>
                  {getStatusText(selectedInvoice.status)}
                </Tag>
              </Descriptions.Item>
            </Descriptions>

            <Divider>发票明细</Divider>

            <Table
              columns={[
                { title: '项目', dataIndex: 'description', key: 'description' },
                { title: '数量', dataIndex: 'quantity', key: 'quantity' },
                { title: '单价', dataIndex: 'unitPrice', key: 'unitPrice', render: (price: number) => `$${price.toFixed(2)}` },
                { title: '小计', dataIndex: 'total', key: 'total', render: (total: number) => `$${total.toFixed(2)}` }
              ]}
              dataSource={selectedInvoice.items}
              pagination={false}
              size="small"
              summary={() => (
                <Table.Summary>
                  <Table.Summary.Row>
                    <Table.Summary.Cell index={0} colSpan={3}>
                      <Text strong>总计</Text>
                    </Table.Summary.Cell>
                    <Table.Summary.Cell index={3}>
                      <Text strong>${selectedInvoice.amount.toFixed(2)}</Text>
                    </Table.Summary.Cell>
                  </Table.Summary.Row>
                </Table.Summary>
              )}
            />
          </div>
        )}
      </Modal>
    </div>
  );
};

export default BillingPage;
```

## CSS 样式定义

```scss
// 订阅计划页面样式
.subscription-plans-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px;

  .page-header {
    text-align: center;
    margin-bottom: 48px;

    h1 {
      font-size: 32px;
      font-weight: bold;
      margin-bottom: 16px;
      background: linear-gradient(135deg, #1890ff, #722ed1);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }

    p {
      font-size: 16px;
      color: #666;
    }
  }

  .plans-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 24px;
    margin-bottom: 48px;
  }

  .plan-card {
    border-radius: 12px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    transition: all 0.3s ease;

    &:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
    }

    &.recommended {
      border: 2px solid #1890ff;
      position: relative;

      &::before {
        content: '推荐';
        position: absolute;
        top: -12px;
        left: 50%;
        transform: translateX(-50%);
        background: #1890ff;
        color: white;
        padding: 4px 16px;
        border-radius: 12px;
        font-size: 12px;
        font-weight: bold;
      }
    }

    &.current {
      border: 2px solid #52c41a;
      background: #f6ffed;
    }

    .plan-header {
      .plan-name {
        font-size: 20px;
        font-weight: bold;
        margin-bottom: 8px;
      }

      .plan-price {
        .price {
          font-size: 28px;
          font-weight: bold;
          color: #1890ff;
        }

        .cycle {
          font-size: 14px;
          color: #666;
        }
      }
    }

    .plan-features {
      .feature-item {
        display: flex;
        align-items: center;
        margin-bottom: 12px;

        .feature-icon {
          color: #52c41a;
          margin-right: 8px;
        }

        .feature-name {
          flex: 1;
          margin-right: 8px;
        }

        .feature-value {
          font-weight: bold;

          &.highlight {
            color: #1890ff;
          }
        }

        .current-usage {
          width: 100%;
          margin-top: 4px;

          .usage-text {
            font-size: 12px;
            color: #666;
          }
        }
      }
    }
  }

  .plans-faq {
    .faq-item {
      margin-bottom: 24px;

      h4 {
        font-size: 16px;
        font-weight: bold;
        margin-bottom: 8px;
        color: #333;
      }

      p {
        color: #666;
        line-height: 1.6;
      }
    }
  }
}

// 用量仪表板样式
.usage-dashboard {
  .dashboard-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 24px;

    h2 {
      margin: 0;
      font-size: 24px;
      font-weight: bold;
    }
  }

  .metrics-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 24px;
    margin-bottom: 24px;
  }

  .usage-metric-card {
    border-radius: 8px;
    transition: all 0.3s ease;

    &:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    &.warning {
      border-left: 4px solid #faad14;
    }

    &.critical {
      border-left: 4px solid #ff4d4f;
    }

    &.exceeded {
      border-left: 4px solid #ff7875;
      background: #fff2f0;
    }

    .metric-header {
      display: flex;
      justify-content: space-between;
      align-items: center;

      .info-icon {
        color: #666;
        cursor: pointer;
      }
    }

    .metric-content {
      .metric-stats {
        margin-bottom: 16px;
      }

      .metric-progress {
        margin-bottom: 16px;

        .progress-info {
          display: flex;
          justify-content: space-between;
          margin-top: 8px;

          .usage-percentage {
            font-weight: bold;
          }

          .status-text {
            color: #666;
            font-size: 12px;
          }
        }
      }

      .metric-trend {
        display: flex;
        align-items: center;
        font-size: 12px;
        color: #666;

        .trend-icon {
          margin-right: 4px;

          &.up {
            color: #ff4d4f;
          }

          &.down {
            color: #52c41a;
          }

          &.stable {
            color: #d9d9d9;
          }
        }
      }
    }
  }

  .chart-controls {
    display: flex;
    align-items: center;
  }

  .usage-recommendations {
    .recommendations-list {
      .recommendation-item {
        padding: 16px;
        border: 1px solid #f0f0f0;
        border-radius: 6px;
        margin-bottom: 12px;

        h4 {
          margin: 0 0 8px 0;
          font-size: 14px;
          font-weight: bold;
        }

        p {
          margin: 0;
          font-size: 12px;
          color: #666;
          line-height: 1.5;
        }
      }
    }
  }
}

// 计费页面样式
.billing-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 24px;

  .page-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 24px;

    h1 {
      margin: 0;
      font-size: 24px;
      font-weight: bold;
    }
  }
}

// 响应式设计
@media (max-width: 768px) {
  .subscription-plans-page .plans-grid {
    grid-template-columns: 1fr;
  }

  .usage-dashboard .metrics-grid {
    grid-template-columns: 1fr;
  }

  .billing-page {
    padding: 16px;

    .page-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 16px;
    }

    & > div:last-child {
      grid-template-columns: 1fr !important;
    }
  }
}
```

## 状态管理集成

```typescript
// Redux store 配置
import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';

interface UsageState {
  metrics: UsageMetric[];
  currentSubscription: any;
  invoices: Invoice[];
  isLoading: boolean;
  error: string | null;
  lastUpdated: string | null;
}

// Async actions
export const fetchUsageMetrics = createAsyncThunk(
  'usage/fetchMetrics',
  async () => {
    const response = await fetch('/api/usage/metrics');
    return response.json();
  }
);

export const trackUsage = createAsyncThunk(
  'usage/track',
  async ({ resource, amount }: { resource: string; amount: number }) => {
    const response = await fetch('/api/usage/track', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ resource, amount })
    });
    return response.json();
  }
);

const usageSlice = createSlice({
  name: 'usage',
  initialState: {
    metrics: [],
    currentSubscription: null,
    invoices: [],
    isLoading: false,
    error: null,
    lastUpdated: null
  } as UsageState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    updateMetric: (state, action) => {
      const { resource, current } = action.payload;
      const metric = state.metrics.find(m => m.resource === resource);
      if (metric) {
        metric.current = current;
        metric.status = determineStatus(current, metric.limit);
      }
    }
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsageMetrics.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(fetchUsageMetrics.fulfilled, (state, action) => {
        state.isLoading = false;
        state.metrics = action.payload;
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchUsageMetrics.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.error.message || 'Failed to load usage metrics';
      });
  }
});

function determineStatus(current: number, limit: number): string {
  const ratio = current / limit;
  if (ratio >= 1) return 'exceeded';
  if (ratio >= 0.95) return 'critical';
  if (ratio >= 0.8) return 'warning';
  return 'normal';
}

export const { clearError, updateMetric } = usageSlice.actions;
export default usageSlice.reducer;
```

## 实时更新机制

```typescript
// WebSocket 连接管理
class UsageRealtimeManager {
  private ws: WebSocket | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 1000;

  constructor(private dispatch: any) {}

  connect() {
    try {
      this.ws = new WebSocket(`${process.env.REACT_APP_WS_URL}/usage`);
      
      this.ws.onopen = () => {
        console.log('Usage WebSocket connected');
        this.reconnectAttempts = 0;
      };

      this.ws.onmessage = (event) => {
        const data = JSON.parse(event.data);
        this.handleMessage(data);
      };

      this.ws.onclose = () => {
        console.log('Usage WebSocket disconnected');
        this.reconnect();
      };

      this.ws.onerror = (error) => {
        console.error('Usage WebSocket error:', error);
      };
    } catch (error) {
      console.error('Failed to connect to usage WebSocket:', error);
      this.reconnect();
    }
  }

  private handleMessage(data: any) {
    switch (data.type) {
      case 'usage_update':
        this.dispatch(updateMetric({
          resource: data.resource,
          current: data.current
        }));
        break;
      
      case 'limit_warning':
        // 显示限制警告
        this.showLimitWarning(data);
        break;
      
      case 'limit_exceeded':
        // 显示限制超出提示
        this.showLimitExceeded(data);
        break;
    }
  }

  private reconnect() {
    if (this.reconnectAttempts < this.maxReconnectAttempts) {
      this.reconnectAttempts++;
      setTimeout(() => {
        console.log(`Reconnecting to usage WebSocket (attempt ${this.reconnectAttempts})`);
        this.connect();
      }, this.reconnectDelay * this.reconnectAttempts);
    }
  }

  private showLimitWarning(data: any) {
    // 实现限制警告逻辑
    console.warn('Usage limit warning:', data);
  }

  private showLimitExceeded(data: any) {
    // 实现限制超出逻辑
    console.error('Usage limit exceeded:', data);
  }

  disconnect() {
    if (this.ws) {
      this.ws.close();
      this.ws = null;
    }
  }
}

export default UsageRealtimeManager;
```

## 总结

这个全面的前端实现指南提供了：

1. **完整的用户界面组件**：订阅计划选择、用量仪表板、限制提示、计费管理
2. **优质的用户体验**：实时更新、智能提醒、个性化建议
3. **响应式设计**：适配各种设备和屏幕尺寸
4. **状态管理**：Redux 集成，统一的数据流管理
5. **实时通信**：WebSocket 连接，实时用量更新
6. **可扩展架构**：模块化设计，易于维护和扩展

这些组件和实现方案将为 BotSharp SaaS 平台提供专业、易用、美观的订阅管理体验，有效提升用户满意度和转化率。
