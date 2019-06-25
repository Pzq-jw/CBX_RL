partition_pattern="Meeting ${1}"
file="README.md"
tempfile="tempfile.md"

echo "The partition pattern is \"${partition_pattern}\""

sed -n '/'"${partition_pattern}"'/,$p' ${file} > ${tempfile}
./md2confluence_gnuSed.sh ${tempfile}
cat ${tempfile} | pbcopy
rm -f ${tempfile}

# --------------------------------------------
# Inside single quote '' everything is preserved literally(i.e., '$a' --> $a)
# Inside double quote "" everything is preserved extendedly(i.e., "$a" --> apple)
# If one want to have the explanation inside the single quote '', it can close
# the quotes, insert something, and then re-single-quote again.
# For example,  sed -n '/'"${partition_pattern}"'/,$p'

# Print all lines after the matched line:
# 		sed -n '/'"${partition_pattern}"'/,$p'
# 		sed -n '/'"${partition_pattern}"'/,$p' | sed '1d' remove the matched line
# Please note that here must be single quote

# Print all lines until the matched line:
# 		sed -n '/'"${partition_pattern}"'/q'
# -n means that sed does not print by default the lines it processes any more.
# $ means that the end of the file
# p means that print current line
# $p means print all lines to end