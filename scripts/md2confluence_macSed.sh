#!/bin/bash

# ---------------------- replace head ----------------------
h5="h5. "
h4="h4. "
h3="h3. "
h2="h2. "
h1="h1. "

sed -i '' "s/^#####\( \)/${h5}/" test.md
sed -i '' "s/^####\( \)/${h4}/" test.md
sed -i '' "s/^###\( \)/${h3}/" test.md
sed -i '' "s/^##\( \)/${h2}/" test.md
sed -i '' "s/^#\( \)/${h1}/" test.md

# ---------------------- replace list ----------------------
l2="-- "
l3="--- "
l4="---- "

sed -i '' "s/			- /${l4}/" test.md
sed -i '' "s/		- /${l3}/" test.md
sed -i '' "s/	- /${l2}/" test.md

# ---------------------- replace bold ----------------------
sed -i '' "s/\`/\*/g" test.md
sed -i '' "s/\*\*/\*/g" test.md

# ---------------------- replace link ----------------------
sed -i '' "s/\(.*\:\/\/.*\))/\1\]/" test.md
sed -i '' "s/\](/\|/" test.md


# Because there may be one or more # in one line, like ### or #####
# we need to substitute the # according to the order from most to least
# to avoid the wrong substitution like ####h1. 

# Need to use sed -i '' with -i flag in Mac sed
# Mac sed not support "//a" flag to add a new line below the match line
# 	because Mac sed not support that \n means a new line.
# Mac sed not support "//{}" format to add another condition

# ----------- Replace only if the string is found in a certain context. ------------

# sed -i '' "s/\(.*\:\/\/.*\))/\1\]/" it means that replace ')' to ']' at
# only the first occurrence, only if there is '://' before the ')'

# sed -i '' "s/\[\(.*\:\/\/.*\)/(\1/" it means that replace '[' to '(' at
# only the first occurrence, only if there is '://' later on the '['

# Both examples can be used by Mac sed, and the before and later condition just need
# to adjust the substitution pattern: 
#	*	If want the condition is found before the matched string, just need to put the condition and 
#		condition regular expression capturing \1 before the pattern stuff.
#	*	If want the condition is found later on the matched string, just need to put the condition and 
#		condition regular expression capturing \1 after the pattern stuff
# The reason is that In sed, using \( \) saves whatever is in the parentheses 
# and you can then access it with \1. This is regular expression(https://www.regular-expressions.info/)

# sed '/ccc/{ s/\(.*aaa: \).*\( bbb\)/\1gotit\2/g }' "2011-07-10 condition ccc aaa: value bbb"
# sed operates only on the line that contains ccc
# Due to the {}, it is only supported by GNU sed!!!

# ----------- Several tricks ------------
# '(' or ')' is not need to escape in match pattern.
# RE: '.' means one any character, '*' means one or more last character
# therefore, '.*' means any contents












