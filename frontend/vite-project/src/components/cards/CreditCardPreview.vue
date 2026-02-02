<template>
  <div class="credit-card-wrapper">
    <div class="credit-card" :class="cardStyleClass">
      <div class="card-header">
        <div class="card-chip">
          <div class="chip-lines"></div>
        </div>
        <div class="card-type">{{ cardType }}</div>
      </div>
      
      <div class="card-number">
        <span class="number-group">{{ maskedNumber[0] }}</span>
        <span class="number-group">{{ maskedNumber[1] }}</span>
        <span class="number-group">{{ maskedNumber[2] }}</span>
        <span class="number-group">{{ maskedNumber[3] }}</span>
      </div>
      
      <div class="card-footer">
        <div class="cardholder-info">
          <p class="cardholder-label">Card Holder</p>
          <p class="cardholder-name">{{ cardholderName }}</p>
        </div>
        <div class="expiration-info">
          <p class="expiration-label">Expires</p>
          <p class="expiration-date">{{ expirationDate }}</p>
        </div>
      </div>
    </div>
    
    <div v-if="showBalance" class="balance-info">
      <div class="balance-row">
        <span class="balance-label">Credit Limit:</span>
        <span class="balance-value">${{ formatCurrency(creditLimit) }}</span>
      </div>
      <div class="balance-row">
        <span class="balance-label">Current Balance:</span>
        <span class="balance-value">${{ formatCurrency(currentBalance) }}</span>
      </div>
    </div>
    
    <div v-if="showActions" class="card-actions">
      <Button 
        variant="secondary" 
        size="small" 
        @click="$emit('update', cardId)"
        class="action-btn"
      >
        Update
      </Button>
      <Button 
        variant="danger" 
        size="small" 
        @click="$emit('delete', cardId)"
        class="action-btn"
      >
        Delete
      </Button>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import Button from '../common/Button.vue';

const props = defineProps({
  cardId: {
    type: [String, Number],
    default: null
  },
  cardType: {
    type: String,
    default: 'VISA'
  },
  cardNumber: {
    type: String,
    default: ''
  },
  cardholderName: {
    type: String,
    default: 'YOUR NAME'
  },
  expirationDate: {
    type: String,
    default: 'MM/YY'
  },
  creditLimit: {
    type: Number,
    default: 0
  },
  currentBalance: {
    type: Number,
    default: 0
  },
  showBalance: {
    type: Boolean,
    default: false
  },
  showActions: {
    type: Boolean,
    default: true
  }
});

defineEmits(['show-details', 'update', 'delete']);

const cardStyleClass = computed(() => {
  const type = props.cardType.toLowerCase();
  
  // Mapper les types de cartes aux classes CSS
  const typeMap = {
    'visa': 'visa',
    'mastercard': 'mastercard', 
    'amex': 'amex',
    'american express': 'amex'
  };
  
  return typeMap[type] || 'visa';
});

const maskedNumber = computed(() => {
  if (!props.cardNumber) {
    return ['••••', '••••', '••••', '••••'];
  }
  
  // Si c'est un numéro partiel du backend (ex: "1234")
  if (props.cardNumber.length === 4) {
    return ['••••', '••••', '••••', props.cardNumber];
  }
  
  // Si c'est un numéro complet (pour la prévisualisation)
  const cleanNumber = props.cardNumber.replace(/\s/g, '');
  const groups = [];
  
  for (let i = 0; i < 4; i++) {
    const start = i * 4;
    const end = start + 4;
    const group = cleanNumber.substring(start, end);
    
    if (group.length === 4) {
      // Masquer les 2 premiers groupes, montrer les 2 derniers
      groups.push(i < 2 ? '••••' : group);
    } else if (group.length > 0) {
      // Groupe partiel
      groups.push(group.padEnd(4, '•'));
    } else {
      groups.push('••••');
    }
  }
  
  return groups;
});

const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(value);
};
</script>

<style scoped>
.credit-card-wrapper {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.credit-card {
  width: 320px;
  height: 200px;
  border-radius: 16px;
  padding: 25px;
  color: white;
  position: relative;
  overflow: hidden;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.credit-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.3);
}

/* Styles de cartes - Force important pour override */
.credit-card.visa {
  background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%) !important;
}

.credit-card.mastercard {
  background: linear-gradient(135deg, #eb001b 0%, #f79e1b 100%) !important;
}

.credit-card.amex {
  background: linear-gradient(135deg, #2e7d32 0%, #4caf50 100%) !important;
}

/* Style par défaut pour les cartes inconnues */
.credit-card:not(.visa):not(.mastercard):not(.amex) {
  background: linear-gradient(135deg, #6c757d 0%, #495057 100%) !important;
}

.balance-info {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 12px;
  border: 1px solid #e9ecef;
}

.balance-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.balance-row:last-child {
  margin-bottom: 0;
}

.balance-label {
  font-size: 14px;
  color: #6c757d;
  font-weight: 500;
}

.balance-value {
  font-size: 14px;
  color: #2c3e50;
  font-weight: 600;
}

.credit-card::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1) 0%, transparent 70%);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 30px;
}

.card-chip {
  width: 40px;
  height: 30px;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.chip-lines {
  width: 25px;
  height: 3px;
  background: #333;
  border-radius: 2px;
  position: relative;
}

.chip-lines::before,
.chip-lines::after {
  content: '';
  position: absolute;
  width: 100%;
  height: 2px;
  background: #333;
  border-radius: 1px;
}

.chip-lines::before {
  top: -6px;
}

.chip-lines::after {
  top: 6px;
}

.card-type {
  font-size: 18px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.card-number {
  display: flex;
  gap: 15px;
  margin-bottom: 30px;
  font-family: 'Courier New', monospace;
  font-size: 18px;
  letter-spacing: 2px;
}

.number-group {
  flex: 1;
  text-align: center;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
}

.cardholder-info,
.expiration-info {
  flex: 1;
}

.cardholder-label,
.expiration-label {
  margin: 0 0 5px 0;
  font-size: 10px;
  text-transform: uppercase;
  letter-spacing: 1px;
  opacity: 0.8;
}

.cardholder-name,
.expiration-date {
  margin: 0;
  font-size: 14px;
  font-weight: 500;
  text-transform: uppercase;
}

.expiration-info {
  text-align: right;
}

.card-actions {
  display: flex;
  gap: 8px;
  justify-content: space-between;
}

.action-btn {
  flex: 1;
  font-size: 12px;
  padding: 8px 12px;
  font-weight: 500;
}

@media (max-width: 768px) {
  .credit-card {
    width: 100%;
    max-width: 320px;
  }
  
  .card-actions {
    flex-wrap: wrap;
    gap: 6px;
  }
  
  .action-btn {
    flex: 1 1 calc(33.333% - 4px);
    min-width: 80px;
  }
}
</style>
