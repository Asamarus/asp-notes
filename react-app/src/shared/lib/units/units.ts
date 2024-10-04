/**
 * Get correct unit name
 */

interface Cases {
  nom: string
  gen: string
  plu: string
}

function units(num: number, cases: Cases) {
  num = Math.abs(num)

  let word = ''

  if (num.toString().indexOf('.') > -1) {
    word = cases.gen
  } else {
    word =
      num % 10 == 1 && num % 100 != 11
        ? cases.nom
        : num % 10 >= 2 && num % 10 <= 4 && (num % 100 < 10 || num % 100 >= 20)
        ? cases.gen
        : cases.plu
  }

  return word
}

export default units
