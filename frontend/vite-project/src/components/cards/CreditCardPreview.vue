<template>
  <div class="credit-card" :class="cardType.toLowerCase()">
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
</template>

<script setup>
defineProps({
  cardType: {
    type: String,
    required: true
  },
  maskedNumber: {
    type: Array,
    required: true
  },
  cardholderName: {
    type: String,
    required: true
  },
  expirationDate: {
    type: String,
    required: true
  }
});
</script>

<style scoped>
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

.credit-card.visa {
  background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
}

.credit-card.mastercard {
  background: linear-gradient(135deg, #eb001b 0%, #f79e1b 100%);
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
</style>
